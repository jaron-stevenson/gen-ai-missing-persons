namespace ui_frontend.Service
{
    public class ProgressService
    {
        private bool showProgress = false;
        public bool ShowProgress
        {
            get => showProgress;
            set
            {
                if (showProgress != value)
                {
                    showProgress = value;
                    if (Notify != null)
                    {
                        Notify?.Invoke();
                    }

                }
            }
        }

        public event Action? Notify;
    }
}
