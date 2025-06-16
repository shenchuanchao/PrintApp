namespace PrintApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new PrintingApp());
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show("����δ�����쳣��" + e.Exception.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show("�������ش���" + ((Exception)e.ExceptionObject).Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

        }
    }
}