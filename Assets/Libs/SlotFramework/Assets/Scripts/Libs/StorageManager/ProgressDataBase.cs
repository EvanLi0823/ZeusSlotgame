using System;
namespace Libs
{
    /// Base class for progress data management in the Slot Framework
    [Serializable]
    public abstract class ProgressDataBase<T> where T : ProgressDataBase<T>
    {
        public abstract void LoadData(T progressData);

        public abstract void SaveData();

        public abstract void ClearData();
    }
}