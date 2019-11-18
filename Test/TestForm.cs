using System.Windows.Forms;

namespace VsToolsTest
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		public string Test1 = "abc";
		public string Test123 = "abc";
		public string Test3333 = "abc";
		public string Test121212 = "abc";

		private void TestForm_Load(object sender, System.EventArgs e)
		{
			if (_isDirty)
			{
				return;
			}  

			if (_isDirty)  
				return;

			if (_isDirty)  return;

			IsDirty = true;
		}

		private void TestForm_KeyDown(object sender, KeyEventArgs e)
		{

		}

		public bool TestMe1()
		{
			if (_isDirty)
				return true;
			return false;
		}

		public bool TestMe2()
		{
			if (_isDirty)
			{
				return true;
			}
			return false;
		}

		public bool TestMe3()
		{
			if (_isDirty)
			{
				_isDirty = true;
				return _isDirty;
			}
			return false;
		}

		public void TestMe4()
		{
			for (int i = 0; i < 2; i++)
			{
				if (_isDirty)
				continue;
			}
			
		}

		private bool _isDirty;
		
		public  bool IsDirty
		{
			get { return true; }
			set { _isDirty = value; }
		}
	}
}
