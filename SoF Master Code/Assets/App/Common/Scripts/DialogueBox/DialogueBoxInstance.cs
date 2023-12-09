using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxInstance : MonoBehaviour
{
	public GameObject containerPanel;
	public Button yesButton;
	public Button noButton;
	public Button downloadButton;
	public Button retryButton;

	public GameObject titlePanel;
	public Text titleLabel;
	public Text textLabel;

	private DialogueBoxController.Type type;

	public delegate void CallBack();
	protected CallBack confirmCallBackFunction;
	protected CallBack cancelCallBackFunction;

	public void ShowMessage(string msg, string title, DialogueBoxController.Type type = DialogueBoxController.Type.Standard,
							CallBack confirmCallBackFunction = null,
							CallBack cancelCallBackFunction = null)
	{
		if (!containerPanel.activeInHierarchy)
		{
			containerPanel.SetActive(true);
		}

		textLabel.text = msg;
		titleLabel.text = title;
		if (title != "")
		{
			titlePanel.SetActive(true);
		} else
		{
			titlePanel.SetActive(false);
		}

		//Reset all
		yesButton.gameObject.SetActive(false);
		noButton.gameObject.SetActive(false);
		downloadButton.gameObject.SetActive(false);
		retryButton.gameObject.SetActive(false);

		switch (type)
		{
            case DialogueBoxController.Type.Standard:
                yesButton.gameObject.SetActive(true);
                break;
            case DialogueBoxController.Type.StandardWithCallBack:
				yesButton.gameObject.SetActive(true);
                if(confirmCallBackFunction != null)
                    this.confirmCallBackFunction = confirmCallBackFunction;
                break;
			case DialogueBoxController.Type.NoButtons:
				break;
			case DialogueBoxController.Type.Confirm:
				//Callback CANNOT take in any params. MUST be just a confim call. Caller should store params on their end.
				yesButton.gameObject.SetActive(true);
				noButton.gameObject.SetActive(true);
				this.confirmCallBackFunction = confirmCallBackFunction;
				this.cancelCallBackFunction = cancelCallBackFunction;
				break;
			case DialogueBoxController.Type.AutoClose:
				//overlayCloseButton.gameObject.SetActive(true);
				Invoke("FadeOutAndClose", 1);
				break;
			case DialogueBoxController.Type.Error:
				msg = MultiLanguage.getInstance().getString("dialog_box_error_message") + msg;
				textLabel.text = msg;
				noButton.gameObject.SetActive(true);
				break;
			case DialogueBoxController.Type.Download:
				textLabel.text = msg;
				downloadButton.gameObject.SetActive(true);
				noButton.gameObject.SetActive(true);
				break;
			case DialogueBoxController.Type.Retry:
				textLabel.text = msg;
				retryButton.gameObject.SetActive(true);
				break;
		}
	}

	private void Confirm()
	{
		yesButton.gameObject.SetActive(false);
		noButton.gameObject.SetActive(false);
		Hide();
		if (this.confirmCallBackFunction != null)
		{
			this.confirmCallBackFunction();
		}
	}

	public void DownloadNewVersionButtonClick()
	{
#if UNITY_ANDROID
		Application.OpenURL(Constants.AndroidDownloadURL);
#elif UNITY_IOS
		Application.OpenURL(Constants.iOSDownloadURL);
#else
		Application.OpenURL(Constants.defaultDownloadURL);
#endif
	}

	public void CancelButtonClick()
	{
		if (this.cancelCallBackFunction != null)
		{
			yesButton.gameObject.SetActive(false);
			noButton.gameObject.SetActive(false);
			this.cancelCallBackFunction();
		}
		HideButtonClick();
	}

	public void FadeOutAndClose()
	{
		//TODO new UI fadeout
		Hide();
	}

	public void HideButtonClick()
	{
		Hide();
	}

	public void Hide()
	{
		containerPanel.SetActive(false);
		DialogueBoxController.Hide();
	}

	public void ConfirmButtonClick()
	{
		Confirm();
	}

}