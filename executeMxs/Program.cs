using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace executeMxs
{
    class Program
    {
        static void Main( string[] args )
        {
            // args[0] : custom user string
            // args[1] : 3dsmax mxseditor active document FilePath


            if ( args.Length < 2 ) return;

            // first check that any 3dsmax instance is running
            var processes = Process.GetProcessesByName( "3dsmax" );

            if ( processes.Length == 0 ) return;




            // now we need to find all mxseditor windows
            var mxseditors = WinAPI.FilterByClassName( WinAPI.GetChildWindows( new IntPtr( 0 ) ), "MXS_SciTEWindow" );


            uint PID = 0;
            string mxseWindowTitle = (string)args.GetValue( 1 );

            // compare passed argument string with mxse window titles and if it matches check process ID to identify correct 3dsmax instance
            foreach ( var mxse in mxseditors )
            {
                if ( WinAPI.GetWindowText( mxse ).StartsWith( mxseWindowTitle ) )
                {

                    WinAPI.GetWindowThreadProcessId( mxse, out PID );
                    break;
                }

            }

            var active3dsMaxProcess = processes.Where( proc => proc.Id == PID ).ToList();


            if ( active3dsMaxProcess.Count == 0 )
            {
                // in case when script isn't saved check process ID of active window

                IntPtr activeWindow = WinAPI.GetForegroundWindow();
                WinAPI.GetWindowThreadProcessId( activeWindow, out PID );

                active3dsMaxProcess = processes.Where( proc => proc.Id == PID ).ToList();

                if ( active3dsMaxProcess.Count == 0 )
                {
                    MessageBox.Show( "Parent process not found." );
                    return;
                }
            }



            var minilistener = WinAPI.FilterByClassName( WinAPI.GetChildWindows( active3dsMaxProcess[ 0 ].MainWindowHandle ), "MXS_Scintilla" );


            if ( minilistener.Count == 0 )
            {
                MessageBox.Show( "No windows found" );
                return;
            }


            int WM_CHAR = 0x0102;
            int VK_RETURN = 0x0D;
            int WM_SETTEXT = 0x000C;


            string src = string.Format( "{0}();", (string)args.GetValue( 0 ) );


            WinAPI.SendMessage( minilistener[ 1 ], WM_SETTEXT, src.Length + 1, src );
            WinAPI.PostMessage( minilistener[ 1 ], WM_CHAR, VK_RETURN, 0 );



        }
    }
}
