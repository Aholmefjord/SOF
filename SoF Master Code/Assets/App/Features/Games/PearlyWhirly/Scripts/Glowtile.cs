using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Glowtile : MonoBehaviour {
    private bool glowdone = false;
    public GameObject stage;
    private int offsetx=9;
    private int offsetz = 3;


    struct Tileinfo
    {
        public bool activetile;
        public string typetile;
        public bool isglowing;
        public bool starttile;
        public int childnumber;
        public bool searchleft;
        public bool searchright;
        public bool searchup;
        public bool searchdown;

    };

    //Tileinfo[,] maparray = new Tileinfo[,] { };
    Tileinfo[,] maparray = new Tileinfo[35,35];
    //this is so that there is excess tile for array index to find
    //shld use base on map max x and y + extra 1 offset to the 4 side at least
    void Start () {
        //chk level
        //shld load from start or update (udpate?) 
        //read the map 
        //tile array 
        // chk shldglow 
        //chk type of tile (non-straight)
        //change shader 
        //change colour

   
        if (GameState.pearlyProg.selectedLevel<30&& glowdone == false)
        {
            glowdone = true;
            //read the map from static find active by gamestatelevel
            //store date in ? 
            //ez to chk if side tile occupy store xy?
            //assuming this is attach to reeflon stage , find child(levelselcted) doesnt work if oneday map change to 1 level based on json 
            //this shld be load from active map then  read the map 
            //loop to store from reeflon  Reefronstage -> pearlyswirtly->static-> tiles 
            if(GameState.pearlyProg.selectedLevel==16)
            {
                stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(0).GetComponent<Renderer>().material.shader = Shader.Find("Particles/Additive");
                stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(1).GetComponent<Renderer>().material.shader = Shader.Find("Particles/Additive");
                //first 2 tile of lvl 17 exception
            }

            int tempi = 0;
            for (int i = 0;i< stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).childCount; ++i)
            {
                //store the pos ?
                //store type/glowed
                bool temp = false;
          
                if (stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).tag == "Start")
                    temp = true;

             
                maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz].activetile = true;
                maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz].isglowing = false;
                if (!maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz].starttile)
                maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz].starttile = temp;
                if (temp == false)
                    maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild((GameState.pearlyProg.selectedLevel)).GetChild(0).GetChild(i).localPosition.z + offsetz].typetile = stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).GetComponent<Renderer>().material.name;

                maparray[(int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx, (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz].childnumber = i;

            
                Debug.Log(tempi+"number="+((int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.x + offsetx)+","+( (int)stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(i).localPosition.z + offsetz));
            tempi+= 1;
                    }
            Debug.Log(maparray);



        ShldGlow();

        }
       




    }


    void ShldGlow()
    {

     
        int startx = 0;
        int starty = 0;
        bool breakout = false;
        for(int j=0;j<(maparray.GetUpperBound(0)+1);j++)
        {
            for(int k =0;k<(maparray.GetUpperBound(1)+1);k++)
            {
                if(maparray[j,k].starttile==true)
                {
                    startx = j;
                    starty = k;
                    breakout = true;
                    break;
                }
            }
            if (breakout == true)
                break;
        }
        // up down left right chk 
        Check4Dir(startx, starty);



    }
   
    void CheckLeft(int leftx, int lefty)
    {
        //reach end re-search
        //keep checking till false
        int templeftx, templefty;
        templeftx = leftx;
        templefty = lefty;
        //temp being reset ?or reset to right value ?
        if (maparray[templeftx - 1, templefty].activetile == false)
        {
            GlowThisTile(templeftx, templefty);
            //research path from end  if this path is new //glowistrue==false b4 that
            Check4Dir(templeftx, templefty);//this xy shld be the one that end not from top
        }
        else if (maparray[templeftx - 1, templefty].activetile == true)
        {
            templeftx -= 1;

            CheckLeft(templeftx, templefty);
        }

    }
    void CheckUp(int upx,int upy)
    {
        //reach end re-search
        //keep checking till false
        int tempupx, tempupy;
        tempupx = upx;
        tempupy = upy;
    
        //temp being reset ?or reset to right value ?
        if (maparray[tempupx , tempupy+1].activetile == false)
        {
         
            GlowThisTile(tempupx, tempupy);
            //research path from end  if this path is new //glowistrue==false b4 that
            Check4Dir(tempupx, tempupy);//this xy shld be the one that end not from top
        }
        else if (maparray[tempupx , tempupy+1].activetile == true)
        {
            tempupy += 1;
       
            CheckUp(tempupx, tempupy);
        }
    }
    void CheckDown (int downx,int downy)
    {
        int tempupx, tempupy;
        tempupx = downx;
        tempupy = downy;
        //temp being reset ?or reset to right value ?
        if (maparray[tempupx , tempupy-1].activetile == false)
        {
            GlowThisTile(tempupx, tempupy);
            //research path from end  if this path is new //glowistrue==false b4 that
         
           Check4Dir(tempupx, tempupy);//this xy shld be the one that end not from top
        }
        else if (maparray[tempupx , tempupy-1].activetile == true)
        {
            tempupy -= 1;

            CheckDown(tempupx, tempupy);
        }
    }
    void CheckRight(int rightx, int righty)
    {
        //reach end re-search
        //keep checking till false
        int temprightx, temprighty;
        temprightx = rightx;
        temprighty = righty;
        //temp being reset ?or reset to right value ?
        if (maparray[temprightx + 1, temprighty].activetile == false)
        {
            GlowThisTile(temprightx, temprighty);
            //research path from end  if this path is new //glowistrue==false b4 that
            Check4Dir(temprightx, temprighty);//this xy shld be the one that end not from top
        }
        else if (maparray[temprightx + 1, temprighty].activetile == true)
        {
            temprightx += 1;
            
            CheckRight(temprightx,temprighty);
        }


        //else return false if active false then glow 
    }
    void Check4Dir(int checkx, int checky)
    {
       //prevent backwardchecking 
       //end if reach glow 
        //x y , x+1 y, x-1 y, x y+1 ,x y-1 
        if (maparray[checkx + 1, checky].activetile == true && maparray[checkx, checky].searchright == false)
        {
            //move right 
            maparray[checkx, checky].searchright = true;
            CheckRight(checkx+1, checky);
           
        }

        if (maparray[checkx - 1, checky].activetile == true && maparray[checkx, checky].searchleft == false)
        {
            //move left 
            maparray[checkx, checky].searchleft = true;
            CheckLeft(checkx - 1, checky);
        }
        if (maparray[checkx , checky+1].activetile == true && maparray[checkx, checky].searchup == false)
        {
            //move up 
            maparray[checkx, checky].searchup = true;
            CheckUp(checkx , checky+1);
        }
        if (maparray[checkx , checky-1].activetile == true && maparray[checkx, checky].searchdown == false)
        {
            //move bottom 
            maparray[checkx, checky].searchdown = true;
            CheckDown(checkx , checky-1);
        }
        //else do nothign? 

    }
    void GlowThisTile(int tilex,int tiley)
    {
    
     
            if (maparray[tilex, tiley].typetile != "BrickStraight (Instance)")
            {
                maparray[tilex, tiley].isglowing = true;
                stage.transform.GetChild(GameState.pearlyProg.selectedLevel).GetChild(0).GetChild(maparray[tilex, tiley].childnumber).GetComponent<Renderer>().material.shader = Shader.Find("Particles/Additive");
            }
     
     
    }

    void Update () {
   
        
    }
  
}
