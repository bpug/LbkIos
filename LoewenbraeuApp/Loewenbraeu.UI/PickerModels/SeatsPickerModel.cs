using System;
using System.Collections.Generic;
using Loewenbraeu.Core;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class SeatsPickerModel : UIPickerViewModel
	{
		public event EventHandler<EventArgs<int>> ValueChanged;
		private List<int> _seats;
		private int _selectedIndex;
		
		public SeatsPickerModel (int maxSeats )
		{
			_seats = new List<int> ();
			for (int i = 2; i < maxSeats +1; i++) {
				_seats.Add (i);
			}
		}
		
		public override float GetComponentWidth (UIPickerView pickerView, int component)
		{
			return 100f;
		}
	
		public override int GetRowsInComponent (UIPickerView pickerView, int component)
		{
			return _seats.Count;
		}
	
		public override int GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			return _seats [row].ToString ();
			
		}
		
		public override void Selected (UIPickerView picker, int row, int component)
		{
			this._selectedIndex = row;
			OnValueChanged (_seats [row]);
		}
		
		public int Seats {
			get { return _seats [this._selectedIndex] ;}
		}
		
		private void OnValueChanged (int seats)
		{
			
			if (this.ValueChanged != null) {
				ValueChanged (this, new EventArgs<int> (seats));
			}
		}
	}
	
}

