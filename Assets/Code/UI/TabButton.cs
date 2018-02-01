using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Jesse.UI
{
	[RequireComponent(typeof(Image))]
	public class TabButton : MonoBehaviour
	{
		private Image image;
		public Color SelectedColor, UnselectedColor;
		public static TabButton SelectedTabButton = null;

		private void Start()
		{
			image = GetComponent<Image>();
		}

		public bool IsSelected
		{
			get
			{
				return SelectedTabButton == this;
			}
			set
			{
				if (value)
				{
					Select();
				}
				else
				{
					Deselect();
				}
			}
		}

		public void Select()
		{
			if (SelectedTabButton != null && SelectedTabButton != this)
				SelectedTabButton.Deselect();
			image.color = SelectedColor;
			SelectedTabButton = this;
		}

		public void Deselect()
		{
			if (SelectedTabButton != null && SelectedTabButton == this)
				SelectedTabButton = null;
			image.color = UnselectedColor;
		}
	}
}
