using System;

public sealed class VideoAsyncTaskEvents {
    public delegate void ResultCallback(bool isSucces, string message);

    public event Action<string> PrintText;
    public event Action<float> OnProgressUpdate;
    public event ResultCallback OnTaskEnd;

    public void Print(string text)
    {
        if (PrintText!=null) PrintText(text);
    }

    public void Update(float progress)
    {
        if (OnProgressUpdate != null) OnProgressUpdate(progress);
    }

    public void End(bool isSuccess, string message)
    {
        if (OnTaskEnd != null) OnTaskEnd(isSuccess, message);
    }
}
