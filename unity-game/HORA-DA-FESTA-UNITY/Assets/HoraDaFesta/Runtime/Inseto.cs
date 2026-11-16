using UnityEngine;
namespace HoraDaFesta {
public class Inseto:MonoBehaviour {
 public Vector3 Target;public int DanceIndex;public bool Crazy;public string Role="quadrilha";
 Arraial scene;Transform body,props;Transform[] limbs=new Transform[6];int instrument=-2;TextMesh nameplate;
 public void Build(Arraial owner,string id){scene=owner;body=new GameObject("Corpo animado").transform;body.SetParent(transform,false);uint hash=2166136261;foreach(char c in id)hash=(hash^c)*16777619;Color color=Color.HSVToRGB((hash%360)/360f,.65f,.9f);
 Part("Abdomen",PrimitiveType.Sphere,body,new Vector3(0,.85f,0),new Vector3(.8f,1,.65f),color);Part("Cabeca",PrimitiveType.Sphere,body,new Vector3(0,1.65f,0),Vector3.one*.65f,color);
 for(int s=-1;s<=1;s+=2){Part("Olho",PrimitiveType.Sphere,body,new Vector3(s*.2f,1.75f,-.26f),Vector3.one*.2f,Color.white);Part("Pupila",PrimitiveType.Sphere,body,new Vector3(s*.2f,1.75f,-.36f),Vector3.one*.09f,Color.black);var antenna=Part("Antena",PrimitiveType.Capsule,body,new Vector3(s*.23f,2.1f,0),new Vector3(.05f,.3f,.05f),Color.black);antenna.transform.localRotation=Quaternion.Euler(0,0,-s*25);Part("Ponta",PrimitiveType.Sphere,body,new Vector3(s*.36f,2.35f,0),Vector3.one*.12f,Arraial.Gold);
 Part("Asa",PrimitiveType.Sphere,body,new Vector3(s*.45f,1.15f,.28f),new Vector3(.5f,.9f,.12f),new Color(.65f,.85f,1));}
 for(int i=0;i<6;i++){limbs[i]=new GameObject("Junta "+i).transform;limbs[i].SetParent(body,false);limbs[i].localPosition=new Vector3(i%2==0?-.35f:.35f,.45f+(i/2)*.3f,0);var leg=Part("Perna",PrimitiveType.Capsule,limbs[i],new Vector3(i%2==0?-.25f:.25f,-.1f,0),new Vector3(.1f,.32f,.1f),Color.black);leg.transform.localRotation=Quaternion.Euler(0,0,i%2==0?-60:60);}
 props=new GameObject("Papel e instrumento").transform;props.SetParent(body,false);var label=new GameObject("Nome");label.transform.SetParent(transform,false);label.transform.localPosition=new Vector3(0,2.7f,0);nameplate=label.AddComponent<TextMesh>();nameplate.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.GetComponent<MeshRenderer>().sharedMaterial=nameplate.font.material;nameplate.text=id.StartsWith("Musico ")?"": "@"+id;nameplate.characterSize=.11f;nameplate.fontSize=32;nameplate.anchor=TextAnchor.MiddleCenter;nameplate.color=Color.white;
 }
 GameObject Part(string n,PrimitiveType t,Transform p,Vector3 v,Vector3 s,Color c)=>scene.Shape(n,t,p,v,s,c);
 public void SetRole(string role,int slot){if(Role==role&&instrument==slot)return;Role=role;instrument=slot;foreach(Transform c in props)Destroy(c.gameObject);
 if(role=="noiva"){Part("Vestido",PrimitiveType.Capsule,props,new Vector3(0,.7f,0),new Vector3(1.25f,.65f,1),Color.white);Part("Veu",PrimitiveType.Cube,props,new Vector3(0,1.8f,.3f),new Vector3(.8f,1,.04f),Color.white);Part("Buque",PrimitiveType.Sphere,props,new Vector3(.5f,1,-.4f),Vector3.one*.4f,Color.magenta);}
 if(role=="noivo"){Part("Cartola",PrimitiveType.Cylinder,props,new Vector3(0,2.15f,0),new Vector3(.55f,.25f,.55f),Color.black);Part("Gravata",PrimitiveType.Cube,props,new Vector3(0,1.3f,-.36f),new Vector3(.35f,.12f,.1f),Color.red);}
 if(role!="banda")return;var brown=new Color(.5f,.22f,.06f);Vector3 front=new Vector3(0,1,-.6f);
 switch(slot){case 0:Part("Sanfona fole",PrimitiveType.Cube,props,front,new Vector3(.8f,.6f,.35f),Color.red);for(int i=0;i<6;i++)Part("Dobra",PrimitiveType.Cube,props,front+new Vector3(-.3f+i*.12f,0,-.2f),new Vector3(.04f,.65f,.1f),Color.white);break;
 case 1:var drum=Part("Zabumba",PrimitiveType.Cylinder,props,front,new Vector3(.9f,.3f,.9f),Color.white);drum.transform.localRotation=Quaternion.Euler(90,0,0);Part("Baqueta",PrimitiveType.Cube,props,new Vector3(.6f,1,-.8f),new Vector3(.05f,.65f,.05f),brown);break;
 case 2:for(int i=0;i<3;i++){var bar=Part("Triangulo",PrimitiveType.Cube,props,front+new Vector3(i==0?-.25f:i==1?.25f:0,i==2?-.2f:0,0),new Vector3(.05f,.6f,.05f),Color.gray);bar.transform.localRotation=Quaternion.Euler(0,0,i==0?-30:i==1?30:90);}break;
 case 3:Part("Sax tubo",PrimitiveType.Capsule,props,front,new Vector3(.15f,.5f,.15f),Arraial.Gold);Part("Sax campana",PrimitiveType.Sphere,props,front+new Vector3(.2f,-.3f,0),new Vector3(.4f,.3f,.3f),Arraial.Gold);break;
 case 4:Part("Teclado",PrimitiveType.Cube,props,front,new Vector3(1.5f,.15f,.5f),Color.black);for(int i=0;i<12;i++)Part("Tecla",PrimitiveType.Cube,props,front+new Vector3(-.65f+i*.12f,.1f,-.05f),new Vector3(.1f,.04f,.35f),i%3==0?Color.black:Color.white);break;
 default:Part("Violao corpo",PrimitiveType.Sphere,props,front,new Vector3(.65f,.85f,.2f),brown);Part("Violao braco",PrimitiveType.Cube,props,front+new Vector3(.35f,.5f,0),new Vector3(.14f,1,.12f),brown);Part("Boca",PrimitiveType.Sphere,props,front+new Vector3(0,0,-.12f),new Vector3(.22f,.22f,.02f),Color.black);break;}
 }
 void Update(){float beat=Time.time*(Crazy?9:3.5f);Vector3 dest=Target;if(Role=="quadrilha"){int pair=DanceIndex/2,side=DanceIndex%2==0?-1:1;int phase=(int)(Time.time/8)%4;float row=(pair%8)*1.7f-6;float column=(pair/8)*4;dest=new Vector3(side*(phase==1?1.1f:3.5f)+column,0,row);if(phase==2){float a=beat*.35f+pair+side*Mathf.PI*.5f;dest=new Vector3(Mathf.Cos(a)*4+column,0,Mathf.Sin(a)*4);}if(phase==3){dest.x=-side*3.5f+column;dest.z=row+Mathf.Sin(beat*.5f)*.5f;}}
 transform.position=Vector3.Lerp(transform.position,dest,Time.deltaTime*3);body.localPosition=new Vector3(0,Mathf.Abs(Mathf.Sin(beat))*.13f,0);body.localRotation=Quaternion.Euler(0,Role=="quadrilha"?Mathf.Sin(beat*.5f)*25:Mathf.Sin(beat)*8,Mathf.Sin(beat)*6);for(int i=0;i<limbs.Length;i++)limbs[i].localRotation=Quaternion.Euler(Mathf.Sin(beat+i)*30,0,Mathf.Sin(beat+i)*18);if(Camera.main)nameplate.transform.rotation=Camera.main.transform.rotation;
 }
}
}
