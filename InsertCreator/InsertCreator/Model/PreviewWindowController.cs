using HgSoftware.InsertCreator.View;
using HgSoftware.InsertCreator.ViewModel;
using System.Linq;

namespace HgSoftware.InsertCreator.Model
{
    internal class PreviewWindowController
    {
        #region Private Fields

        private readonly PreviewViewModel _previewViewModel;
        private readonly PreView _window = new PreView();
        private int _selectedMonitorIndex = 1;

        #endregion Private Fields

        #region Public Constructors

        public PreviewWindowController(PreviewViewModel vm)
        {
            _previewViewModel = vm;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Show()
        {
            if (SetWindow())
            {
                _window.Show();
            }
        }

        public void SetSelectedMonitor(int monitorIndex)
        {
            _selectedMonitorIndex = monitorIndex;
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Close()
        {
            _window.Close();
        }

        internal void Update(bool state)
        {
            if (state && SetWindow())
            {
                _window.Show();
                return;
            }

            _window.Hide();
        }

        #endregion Internal Methods

        #region Private Methods

        private bool SetWindow()
        {
            var screens = System.Windows.Forms.Screen.AllScreens.ToList();

            if (screens.Count < 2)
                return false;

            _window.DataContext = _previewViewModel;
            _window.ShowInTaskbar = false;
            _window.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            _window.WindowState = System.Windows.WindowState.Normal;

            var screen = _selectedMonitorIndex >= 0 && _selectedMonitorIndex < screens.Count
                ? screens[_selectedMonitorIndex]
                : screens.FirstOrDefault(x => !x.Primary) ?? screens[1];
            System.Drawing.Rectangle r = screen.WorkingArea;

            _window.Top = r.Top;
            _window.Left = r.Left;
            _window.Width = r.Width;
            _window.Height = r.Height;
            _window.WindowState = System.Windows.WindowState.Maximized;
            return true;
        }

        #endregion Private Methods
    }
}
