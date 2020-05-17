using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class effectSpawn : MonoBehaviour {
	
	
	public GameObject[] ListaEffetti;
	public float[] Offsets;
	public string[] Descrizioni;	
	private GameObject effect;
	private Ray ray;
    private RaycastHit hit;
	private int listaLength;
	private int id;
	private GUIStyle customText;
	public Color textColor;
	public int fontSize;
	public int textPositionX;
	public int textPositionY;	
	private GameObject instance;
	public bool DontDestroyOldObjectOnNew;
	
	private List<GameObject> oldObject;
	
	void Awake()
	{
	textColor=Color.black;	
	customText=new GUIStyle();
	this.customText.normal.textColor=textColor;	
	this.customText.fontSize=fontSize;
	this.customText.padding.left=textPositionX;
	this.customText.padding.top=textPositionY;
	oldObject= new List<GameObject>();
	}
	
	
	void Start () {
	id=0;		
	effect=ListaEffetti[id];
	listaLength=ListaEffetti.Length;	
	
		
	}
	
	// Update is called once per frame
	void Update () {
	
		getImputMouse();
		getInputKey();
	}
	
	void getImputMouse()
	{
		effect=ListaEffetti[id];
        if (Input.GetButtonDown ("Fire1")) {
	    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	     if (Physics.Raycast(ray, out hit, 1000)){
		Debug.Log(hit.collider);
		if (DontDestroyOldObjectOnNew==false){
			if (oldObject.Count!=0)
			destroyOldObject();			
		}
				
		Vector3 newPos=new Vector3(hit.point.x,Offsets[id],hit.point.z);		
        instance=Instantiate (effect, newPos,Quaternion.Euler(0,0,0)) as GameObject;
		oldObject.Add(instance);		
		}	 
    }
	}
	
	void getInputKey()
	{

        if (Input.GetKeyDown(KeyCode.Space)){
		if (DontDestroyOldObjectOnNew==false){
			if (oldObject.Count!=0)
			destroyOldObject();			
		}	
		id+=1;
		if(id>=listaLength)
		{id=0;}	

		}
	}
	
	void destroyOldObject()
	{
		foreach(var obj in oldObject)
			Object.Destroy(obj);
	}
	
	void OnGUI()		
	{
	this.customText.normal.textColor=textColor;	
	this.customText.fontSize=fontSize;
	this.customText.padding.left=textPositionX;
	this.customText.padding.top=textPositionY;	
	GUI.Label(new Rect(1,1,Screen.width,Screen.height),Descrizioni[id],this.customText);
	}
	
	
}
