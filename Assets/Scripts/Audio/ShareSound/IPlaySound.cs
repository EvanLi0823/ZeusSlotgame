namespace Libs
{
    public interface IPlaySound
    {
        void Play(string name, bool loop = true, float value = 1f, float time = 0);
        void Pause();
        void Stop();
    }



}