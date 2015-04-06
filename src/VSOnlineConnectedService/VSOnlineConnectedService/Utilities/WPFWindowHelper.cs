using System;
using System.Windows;
using System.Windows.Forms;
namespace VSOnlineConnectedService.Utilities
{
    public class WPFWindowHelper : IWin32Window
    {
        public WPFWindowHelper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;

        /// <summary>
        /// Given a framework element, find the top most parent Window
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Window GetTopWindow(DependencyObject element)
        {
                DependencyObject parent = element;
                while (!(parent is Window))
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }
                return (Window)parent;
        }

        /// <summary>
        /// For Win32 Interop, such as the TeamProjectPicker or FileOpen dialog, which needs a IWin32Window as to set the owner of a dailog
        /// </summary>
        /// <param name="element">A framework element used to find the WPF Parent window</param>
        /// <returns>a Win32WIndow to be used as the Owner for other dialog windows</returns>
        public static IWin32Window GetIWin32ParentWindow(DependencyObject element)
        {
            return new WPFWindowHelper(new System.Windows.Interop.WindowInteropHelper(GetTopWindow(element)).Handle);
        }
    }
}
