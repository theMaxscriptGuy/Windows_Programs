using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class packer : MonoBehaviour {

	enum PlatformType
	{
		ios,
		android,
		pc
	}

	public InformationClass ProcessInfo;

	bool loaded = false;

	List<Texture2D> _tex = new List<Texture2D>();
	int currentFile = 0;

	bool processing = false;
	bool makeAtlas = false;
	bool atlasMade = false;

	//Texture2D img;

	bool BEGINPROCESSING = false;

	void OnGUI()
	{
		if(loaded)
		{
			GUI.DrawTexture(new Rect(10,10,_tex[0].width,_tex[0].height),_tex[0]);
		}
		GUI.TextArea (new Rect (10, 10, 100, 50), currentFile.ToString ());
	}

	// Use this for initialization
	void Start () {

		StartCoroutine (getFileList ());
	}
	
	// Update is called once per frame
	void Update () {
		if(BEGINPROCESSING)
		{
			if(currentFile < ProcessInfo.Files.Length-1)
			{
				if(!processing)
					StartCoroutine(pack(_tex, ProcessInfo.Files[currentFile]));
			}
			else
			{
				if(!atlasMade)
					makeAtlas = true;
			}

			if(makeAtlas)
			{
				makeAtlas = false;
				atlasMade = true;
				MakeAtlas(PlatformType.android);
				MakeAtlas(PlatformType.ios);
				MakeAtlas(PlatformType.pc);
				Application.Quit();
			}
		}
	}

	private void MakeAtlas(PlatformType pt)
	{
		Texture2D[] texs = new Texture2D[_tex.Count];
		int area = 0;
		for(int i =0; i < _tex.Count; i++)
		{
			texs[i] = _tex[i];
			area += (texs[i].width * texs[i].height);
		}
		int imageSize = Mathf.ClosestPowerOfTwo((int)Mathf.Sqrt((int)(area * 1.15f)));
		Texture2D imgAtlas = new Texture2D (0, 0);
		AtlasInfo info = new AtlasInfo ();

		if(pt == PlatformType.ios)
		{
			imgAtlas = new Texture2D(imageSize,imageSize,TextureFormat.PVRTC_RGBA4,false);
			info = new AtlasInfo( imgAtlas.PackTextures(texs,2,imageSize));
		}
		else if(pt == PlatformType.android)
		{
			imgAtlas = new Texture2D(imageSize,imageSize, TextureFormat.ASTC_RGBA_8x8,false);
			info = new AtlasInfo( imgAtlas.PackTextures(texs,2,imageSize));
		}
		else if(pt == PlatformType.pc)
		{
			imgAtlas = new Texture2D(imageSize,imageSize, TextureFormat.DXT5,false);
			info = new AtlasInfo( imgAtlas.PackTextures(texs,2,imageSize));
		}

		byte[] bytes = imgAtlas.EncodeToPNG();

		if(!Directory.Exists(ProcessInfo.Path + "/" + pt.ToString()))
		{
			Directory.CreateDirectory (ProcessInfo.Path + "/" + pt.ToString());
		}
		
		if(File.Exists(ProcessInfo.Path + "/" + pt.ToString() + "/" + ProcessInfo.FileName + ".png"))
		{
			File.Delete(ProcessInfo.Path + "/" + pt.ToString() + "/" + ProcessInfo.FileName + ".png");
		}
		File.WriteAllBytes(ProcessInfo.Path + "/" + pt.ToString() + "/" + ProcessInfo.FileName + ".png", bytes);

		string infoJSON = JsonConvert.SerializeObject (info);



		/*StreamWriter sw = new StreamWriter (ProcessInfo.Path + "/" + pt.ToString() + "/" + ProcessInfo.FileName + ".txt");
		sw.Write (infoJSON);
		sw.Close ();
		*/
	}

	IEnumerator getFileList()
	{
		WWW www = new WWW ("file:///" + Application.streamingAssetsPath + "/files.txt");
		while(!www.isDone)
		{
			yield return null;
		}
		if(www.isDone)
		{
			ProcessInfo = JsonConvert.DeserializeObject<InformationClass>(www.text);
			BEGINPROCESSING = true;
		}
	}

	IEnumerator pack(List<Texture2D> list, string file)
	{
		Debug.Log (file);
		//WWW www = new WWW ("file:///" + Application.streamingAssetsPath + "/images/" + file + ".png");
		WWW www = new WWW ("file:///" + file);
		while(!www.isDone)
		{
			processing = true;
			yield return null;
		}
		if(www.isDone)
		{
			Texture2D tex = new Texture2D (www.texture.width,www.texture.height);
			www.LoadImageIntoTexture(tex);
			list.Add(tex);
			loaded = true;
			currentFile+=1;
			processing = false;
		}
	}
}

public class InformationClass
{
	public string FileName;
	public string Path;
	public string[] Files;
	
	public InformationClass()
	{
		
	}
	
	public InformationClass(string fName, string path, string[] files)
	{
		FileName = fName;
		Path = path;
		Files = files;
	}
}

public class AtlasInfo
{
	public List<Rectangle> Data;
	public AtlasInfo()
	{
		Data = new List<Rectangle> ();
	}
	public AtlasInfo(Rect[] rectangles)
	{
		Data = new List<Rectangle> ();
		foreach(Rect r in rectangles)
		{
			Data.Add (new Rectangle(r.x,r.y,r.width,r.height));
		}
	}
}

public class Rectangle
{
	public float x,y,w,h;
	public Rectangle()
	{

	}
	public Rectangle(float _x,float _y,float _w,float _h)
	{
		x = _x;
		y = _y;
		w = _w;
		h = _h;
	}
}
