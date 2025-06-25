namespace Libs {

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class FileMessage {
        public const string ReadFileOperation = "ReadFileOperation";
        public const string WriteFileOperation = "WriteFileOperation";
        
        public string FileOperation { get; set; }
        public string FileFullPath { get; set; }
        public string FileContent { get; set; }
        public object UserInfo { get; set; }

        public FileMessage(string fileOperation, string fileFullPath) {
            FileOperation = fileOperation;
            FileFullPath = fileFullPath;
        }

        public FileMessage(string fileOperation, string fileFullPath, object userInfo) {
            FileOperation = fileOperation;
            FileFullPath = fileFullPath;
            UserInfo = userInfo;
        }
    }

    public class ReadFileMessage : FileMessage {
        public delegate void OnReadFinished(string fileFullPath, string fileContent);
        public delegate void OnReadFinishedWithUserInfo(string fileFullPath, string fileContent, object userInfo);
        public OnReadFinished readFinishedDelegate;
        public OnReadFinishedWithUserInfo readFinishedWithUserInfoDelegate;

        public ReadFileMessage(string fileFullPath, OnReadFinished readDelegate) : 
            base(FileMessage.ReadFileOperation, fileFullPath) {
            readFinishedDelegate += readDelegate;
        }

        public ReadFileMessage(string fileFullPath, object userInfo, OnReadFinishedWithUserInfo readDelegate) :
            base(FileMessage.ReadFileOperation, fileFullPath, userInfo) {
            readFinishedWithUserInfoDelegate += readDelegate;
        }
    }

    public class WriteFileMessage : FileMessage {
        public delegate void OnWriteFinished(string fileFullPath, string fileContentWritten);
        public delegate void OnWriteFinishedWithUserInfo(string fileFullPath, string fileContentWritten, object userInfo);
        public OnWriteFinished writeFinishedDelegate;
        public OnWriteFinishedWithUserInfo writeFinishedWithUserInfoDelegate;

        public WriteFileMessage(string fileFullPath, string fileContentToWrite, OnWriteFinished writeDelegate) :
            base(FileMessage.WriteFileOperation, fileFullPath) {
            FileContent = fileContentToWrite;
            writeFinishedDelegate += writeDelegate;
        }

        public WriteFileMessage(string fileFullPath, string fileContentToWrite, object userInfo, OnWriteFinishedWithUserInfo writeDelegate) :
            base(FileMessage.WriteFileOperation, fileFullPath, userInfo) {
            FileContent = fileContentToWrite;
            writeFinishedWithUserInfoDelegate += writeDelegate;
        }
    }

    public class FileManager {
        
        private static Queue fileSyncOperationQueue = Queue.Synchronized(new Queue());
        private static Queue fileDispatchQueue = Queue.Synchronized(new Queue());

        public static Thread thread { get; set; }

        public static AutoResetEvent autoResetCloseEvent = new AutoResetEvent(false);
        public static AutoResetEvent autoResetFileOperationEvent = new AutoResetEvent(false);

        public static WaitHandle[] waitHandles = new WaitHandle[] { autoResetCloseEvent, autoResetFileOperationEvent };

        public static void StartFileManager() {
            if (thread == null) {
                autoResetCloseEvent.Reset();
                thread = new Thread(new ThreadStart(FileManagerMainThreadProc));
                thread.Start();
            }
        }

        public static void StopFileManager() {
            if (thread != null) {
                autoResetCloseEvent.Set();
            }
        }

        public static void AddFileOperation(FileMessage fileMessage) {
            lock(fileSyncOperationQueue) {
                fileSyncOperationQueue.Enqueue(fileMessage);
                autoResetFileOperationEvent.Set();
            }
        }

        public static void FileManagerMainThreadProc() {
            Int32 waitRet = WaitHandle.WaitTimeout;
            while (waitRet == WaitHandle.WaitTimeout || waitRet == 1) {
                waitRet = WaitHandle.WaitAny(FileManager.waitHandles, 2000);

                // autoResetCloseEvent
                if (waitRet == 0) {
                    FileManager.thread = null;
                    return;
                }

                // autoResetFileOperationEvent
                if (waitRet == 1) {
                    while (FileManager.fileSyncOperationQueue.Count > 0) {
                        FileMessage message = null;
                        lock (FileManager.fileSyncOperationQueue) {
                            if (FileManager.fileSyncOperationQueue.Count > 0) {
                                message = FileManager.fileSyncOperationQueue.Dequeue() as FileMessage;
                            }
                        }

                        if (message != null) {
                            switch (message.FileOperation) {
                                case FileMessage.ReadFileOperation:
                                    ReadFileMessage readFileMessage = message as ReadFileMessage;
                                    FileManager.ReadFile(readFileMessage);
                                    break;
                                case FileMessage.WriteFileOperation:
                                    WriteFileMessage writeFileMessage = message as WriteFileMessage;
                                    FileManager.WriteFile(writeFileMessage);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static void ReadFile(ReadFileMessage message) {
            if (message == null) {
                return;
            }

            try {
                if (File.Exists(message.FileFullPath) == false) {
                    goto ReadError;
                }

                using (StreamReader reader = new StreamReader(message.FileFullPath)) {
                    message.FileContent = reader.ReadToEnd();
                    goto ReadError;
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            ReadError:
            EnqueueFileDispatch(message);
        }

        private static void WriteFile(WriteFileMessage message) {
            if (message == null) {
                return;
            }

            try {
                FileInfo fileInfo = new FileInfo(message.FileFullPath);
                DirectoryInfo directoryInfo = fileInfo.Directory;
                if (directoryInfo.Exists == false) {
                    directoryInfo.Create();
                }

                using (FileStream stream = File.Exists(message.FileFullPath) ?
                    new FileStream(message.FileFullPath, FileMode.Truncate, FileAccess.Write) :
                    new FileStream(message.FileFullPath, FileMode.Create, FileAccess.Write)) {
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.Write(message.FileContent);
                        writer.Flush();
                        goto WriteError;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            WriteError:
            EnqueueFileDispatch(message);
        }

        private static void EnqueueFileDispatch(FileMessage message) {
            if (message == null) {
                return;
            }

            lock(fileDispatchQueue) {
                fileDispatchQueue.Enqueue(message);
            }
        }

        private static void DequeueFileDispatch() {
            while (FileManager.fileDispatchQueue.Count > 0) {
                FileMessage message = null;
                lock (fileDispatchQueue) {
                    if (FileManager.fileDispatchQueue.Count > 0) {
                        message = fileDispatchQueue.Dequeue() as FileMessage;
                    }
                }

                if (message != null) {
                    switch(message.FileOperation) {
                        case FileMessage.ReadFileOperation: {
                                ReadFileMessage rmessage = message as ReadFileMessage;
                                if (rmessage.readFinishedDelegate != null) {
                                    rmessage.readFinishedDelegate(rmessage.FileFullPath, rmessage.FileContent);
                                } else if (rmessage.readFinishedWithUserInfoDelegate != null) {
                                    rmessage.readFinishedWithUserInfoDelegate(rmessage.FileFullPath, rmessage.FileContent, rmessage.UserInfo);
                                }
                            }
                            break;

                        case FileMessage.WriteFileOperation: {
                                WriteFileMessage wmessage = message as WriteFileMessage;
                                if (wmessage.writeFinishedDelegate != null) {
                                    wmessage.writeFinishedDelegate(wmessage.FileFullPath, wmessage.FileContent);
                                } else if (wmessage.writeFinishedWithUserInfoDelegate != null) {
                                    wmessage.writeFinishedWithUserInfoDelegate(wmessage.FileFullPath, wmessage.FileContent, wmessage.UserInfo);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        // Add this method for change the thread from file-operation thread to main thread.
        // Preventing multi-thread issues.
        public static void OnFileDispatch() {
            if (FileManager.fileDispatchQueue.Count == 0) {
                return;
            }

            DequeueFileDispatch();
        }
    }
}