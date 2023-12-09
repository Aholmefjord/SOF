using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PhotoGallery : MonoBehaviour
{
    public GameObject galleryPanel, galleryScroll, enlargePanel;
    public GameObject textureTemplate;
    public GameObject captureButton;
    public GameObject galleryButton;
    private List<GameObject> galleryList = new List<GameObject>();
    private Vector2 gallerySizeDelta;
    private Vector3 originalGalleryIconPos, originalGalleryIconScale;
    //Use this for initialization

    public void Start()
    {

       gallerySizeDelta = galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        //originalGalleryIconPos = new Vector3(23.47f, 0.0f, 0.0f);
        //originalGalleryIconScale = new Vector3(0.6156662f, 0.6156662f, 1.0f);
       originalGalleryIconPos = galleryButton.transform.GetChild(0).GetComponent<RectTransform>().position;

       originalGalleryIconScale = galleryButton.transform.GetChild(0).GetComponent<RectTransform>().localScale;


       LoadGalleryButtonImage();
    }

    //Update is called once per frame
    public void Update()
    {

    }

    public void UpdateGallery()
    {
        LoadGalleryButtonImage();
    }

    public static bool FindImageInGallery(string filename)
    {
        return false;
#if UNITY_EDITOR
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        foreach (string photo in files)
        {
            if (photo.Contains(filename))
                match = true;
        }
        if (!match)
            return false;
        else
            return true;
#else
#if UNITY_IPHONE
        string folder = GetiPhoneDocumentsPath();
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        foreach (string photo in files)
        {
            if (photo.Contains(filename))
                match = true;
        }
        if (!match)
            return false;
        else
            return true;
#elif UNITY_ANDROID
        string folder = "/sdcard/Pictures";
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        foreach (string photo in files)
        {
            if (photo.Contains(filename))
                match = true;
        }
        if (!match)
            return false;
        else
            return true;
#endif
#endif
    }

    private void LoadGalleryButtonImage()
    {
#if UNITY_EDITOR
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        List<string> photoList = new List<string>();
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                match = true;
                photoList.Add(photo);
            }
        }
        if (match)
        {
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = gallerySizeDelta;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().localScale = originalGalleryIconScale;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().position = originalGalleryIconPos;
            LoadImageFromDevice(photoList[(photoList.Count - 1)], galleryButton.GetComponentInChildren<RawImage>());
            galleryButton.transform.GetChild(0).name = photoList[(photoList.Count - 1)];

            galleryButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            galleryButton.GetComponent<Button>().interactable = false;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = galleryButton.GetComponent<RectTransform>().sizeDelta;
            galleryButton.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
            galleryButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
#else
#if UNITY_IPHONE
        string folder = GetiPhoneDocumentsPath();
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        List<string> photoList = new List<string>();
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                match = true;
                photoList.Add(photo);
            }
        }
        if (match)
        {
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = gallerySizeDelta;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().localScale = originalGalleryIconScale;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().position = originalGalleryIconPos;
            LoadImageFromDevice(photoList[(photoList.Count - 1)], galleryButton.GetComponentInChildren<RawImage>());
            galleryButton.transform.GetChild(0).name = photoList[(photoList.Count - 1)];

            galleryButton.GetComponent<Button>().interactable = true;
        }
        else
        { 
            galleryButton.GetComponent<Button>().interactable = false;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = galleryButton.GetComponent<RectTransform>().sizeDelta;
            galleryButton.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
            galleryButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
#elif UNITY_ANDROID
        string folder = "/sdcard/Pictures";
        string[] files = Directory.GetFiles(folder);
        bool match = false;
        List<string> photoList = new List<string>();
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                match = true;
                photoList.Add(photo);
            }
        }
        if (match)
        {
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = gallerySizeDelta;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().localScale = originalGalleryIconScale;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().position = originalGalleryIconPos;
            LoadImageFromDevice(photoList[(photoList.Count - 1)], galleryButton.GetComponentInChildren<RawImage>());
            galleryButton.transform.GetChild(0).name = photoList[(photoList.Count - 1)];

            galleryButton.GetComponent<Button>().interactable = true;
        }
        else
        { 
            galleryButton.GetComponent<Button>().interactable = false;
            galleryButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = galleryButton.GetComponent<RectTransform>().sizeDelta;
            galleryButton.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
            galleryButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
#endif
#endif
    }

    public static void LoadImageFromDevice(string image, RawImage imageToChange)
    {
#if UNITY_EDITOR
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains(image))
            {
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                imageToChange.texture = newTexture;
            }
            else
            { }
        }
#else
#if UNITY_IPHONE
        string folder = GetiPhoneDocumentsPath();
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains(image))
            {
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                imageToChange.texture = newTexture;
            }
            else
            { }
        }
#elif UNITY_ANDROID
        string folder = "/sdcard/Pictures";
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains(image))
            {
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                imageToChange.texture = newTexture;
            }
            else
            { }
        }
#endif
#endif
    }

    private void LoadImagesFromDevice()
    {
        try
        {
            if (galleryList.Count != 0)
            {
                for (int i = 0; i <= galleryList.Count; i++)
                {
                    try { Destroy(galleryList[i]); } catch (Exception e) { Debug.LogError(e); }
                }
            }
        }
        catch (Exception e) { }
#if UNITY_EDITOR
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                Debug.Log("file match");
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                GameObject newImage = Instantiate(textureTemplate, textureTemplate.transform.parent);
                newImage.GetComponent<RawImage>().texture = newTexture;
                newImage.SetActive(true);
                galleryList.Add(newImage);
            }
        }
#else
#if UNITY_IPHONE
        string folder = GetiPhoneDocumentsPath();
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                Debug.Log("file match");
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                GameObject newImage = Instantiate(textureTemplate, textureTemplate.transform.parent);
                newImage.GetComponent<RawImage>().texture = newTexture;
                newImage.SetActive(true);
                galleryList.Add(newImage);
            }
        }
#elif UNITY_ANDROID
        string folder = "/sdcard/Pictures";
        string[] files = Directory.GetFiles(folder);
        foreach (string photo in files)
        {
            if (photo.Contains("BuddyPhoto"))
            {
                Debug.Log("file match");
                Texture2D newTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                byte[] fileData = File.ReadAllBytes(photo);
                newTexture.LoadImage(fileData);

                GameObject newImage = Instantiate(textureTemplate, textureTemplate.transform.parent);
                newImage.GetComponent<RawImage>().texture = newTexture;
                newImage.SetActive(true);
                galleryList.Add(newImage);
            }
        }
#endif
#endif
    }

    public void OpenGalleryPanel()
    {
        galleryPanel.SetActive(true);
        captureButton.SetActive(false);
        galleryButton.SetActive(false);
        LoadImagesFromDevice();
    }

    public void CloseGalleryPanel()
    {
        galleryPanel.SetActive(false);
        captureButton.SetActive(true);
        galleryButton.SetActive(true);
    }

    public void EnlargeImage(GameObject textureToRender)
    {
        enlargePanel.GetComponent<RawImage>().texture = textureToRender.GetComponent<RawImage>().texture;
        enlargePanel.SetActive(true);
        galleryScroll.SetActive(false);
    }

    public void MinimizeImage()
    {
        galleryScroll.SetActive(true);
        enlargePanel.SetActive(false);
    }
}
