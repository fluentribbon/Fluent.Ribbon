namespace FluentTest
{
    /// <summary>
    /// Forwards to the available datepicker class
    /// </summary>
    public class DatePicker 
#if NET35
       : Microsoft.Windows.Controls.DatePicker
#else
       : System.Windows.Controls.DatePicker
#endif
    {
    }
}