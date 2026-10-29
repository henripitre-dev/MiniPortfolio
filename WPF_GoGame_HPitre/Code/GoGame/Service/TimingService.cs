using System.Threading.Tasks;

namespace GoGame.Service
{
    public class TimingService
    {
        private bool _running;

        public async Task CountAsync(Action<int> setSeconds)
        {
            _running = true;
            int seconds = 0;

            while (_running)
            {
                await Task.Delay(1000);
                seconds++;

                setSeconds(seconds);
            }
        }

        public void Stop() =>
            _running = false;

    }
}