using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace executeMxs
{
    static class WinAPI
    {

        [DllImport( "user32" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool EnumChildWindows( IntPtr window, EnumWindowProc callback, IntPtr i );

        /// <summary>
        /// Returns a list of child windows
        /// </summary>
        /// <param name="parent">Parent of the windows to return</param>
        /// <returns>List of child windows</returns>
        public static List<IntPtr> GetChildWindows( IntPtr parent )
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc( result );
            try
            {
                EnumWindowProc childProc = new EnumWindowProc( EnumWindow );
                EnumChildWindows( parent, childProc, GCHandle.ToIntPtr( listHandle ) );
            }
            finally
            {
                if ( listHandle.IsAllocated )
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow( IntPtr handle, IntPtr pointer )
        {
            GCHandle gch = GCHandle.FromIntPtr( pointer );
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if ( list == null )
            {
                throw new InvalidCastException( "GCHandle Target could not be cast as List<IntPtr>" );
            }
            list.Add( handle );
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }

        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to bail.</returns>
        public delegate bool EnumWindowProc( IntPtr hWnd, IntPtr parameter );

        
        public static List<IntPtr> FilterByClassName( List<IntPtr> ptrs, string className )
        {
            return ptrs.Where( ptr =>
            {

                StringBuilder ClassNameSB = new StringBuilder( 256 );
                int nRet = GetClassName( ptr, ClassNameSB, ClassNameSB.Capacity );
                if ( nRet != 0 )
                {
                    return string.Compare( ClassNameSB.ToString(), className, true, CultureInfo.InvariantCulture ) == 0;

                }

                return false;
            }

            ).ToList();

        }







        public static string GetWindowText( IntPtr hWnd )
        {
            int len = GetWindowTextLength( hWnd ) + 1;
            StringBuilder sb = new StringBuilder( len );
            len = GetWindowText( hWnd, sb, len );
            return sb.ToString( 0, len );
        }

        [DllImport( "user32.dll", SetLastError = true )]
        static extern int GetWindowTextLength( IntPtr hWnd );
        [DllImport( "user32.dll", SetLastError = true )]
        static extern int GetWindowText( IntPtr hWnd, StringBuilder lpString, int nMaxCount );                

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern bool SetWindowText( IntPtr hwnd, String lpString );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern int SendMessage( IntPtr hWnd, int wMsg, int wParam, string lParam );
        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern int PostMessage( IntPtr hWnd, int wMsg, int wParam, [MarshalAs( UnmanagedType.LPStr )] string lParam );



        [DllImport( "user32.dll" )]
        public static extern int PostMessage( IntPtr hWnd, int wMsg, long wParam, long lParam );

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint processId );

        [DllImport( "user32.dll" )]
        public static extern bool SetForegroundWindow( IntPtr hWnd );
        [DllImport( "user32.dll" )]
        public static extern IntPtr GetForegroundWindow();


    }
}
