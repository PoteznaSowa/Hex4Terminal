using System;
using System.Collections.Generic;
using System.Text;

namespace Hex4Terminal {
	public enum DialogResult {
		None,
		OK,
		Cancel,
		Yes,
		No
	}
	public interface IDialog {
		DialogResult Show();
	}
}
