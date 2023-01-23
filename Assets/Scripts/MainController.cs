using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public GameObject block;
	public GameObject ghostBlock;
	public Sprite purp;
	public Sprite redd;
	public Sprite blue;
	public Sprite aqua;
	public Sprite yell;
	public Sprite blak;
	public Sprite grin;
	public Sprite pank;
	public Sprite snow;
	public Sprite tech;

	//the horizonal distance between two hexes
	//the hexes are .64 apart between their centers, but they are never horizonally aligned
	static float h = .55426f;

    Hex rockStar; //center stage
    List<Hex> band = new List<Hex>(); //band of teenagers trying to get seats
	List<Sprite> wardrobe = new List<Sprite>(); //list of sprites to color each row
	List<List<Hex>> attend = new List<List<Hex>>(); //list of hexes grouped by distance from center
	int countdown = 5000;	//milliseconds between every movement
	String dir = "dow"; //the direction the band of hexes is falling, which starts at the top going down

    // Start is called before the first frame update
	// Fills wardrobe
	// Creates and connects the board nodes
	// Creates starting state
    void Start() {
		//create the wardrobe that is used to randomly select a color for the next circle of blocks to be painted
		wardrobe.Add   (                                            purp );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), redd );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), blue );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), aqua );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), yell );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), blak );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), grin );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), pank );
		wardrobe.Insert(UnityEngine.Random.Range(0,wardrobe.Count), snow );

		for (int i = 0; i < 9; i++) attend.Add(new List<Hex>());

        Hex lastCol = null;    //keeps track of the column to the left
        Hex lastHex = null;    //keeps track of the hex to the bottom left
        
		//Instantiate all the Hexes, which represent the board locations
        //for columns -9 through +9
        for (int i = -9; i < 10; i++)
        {
            Hex hex = new Hex();
            hex.dive = i;
            hex.rise = 9 - (hex.dive > 0? hex.dive : 0);
            hex.fall = hex.dive + 9 - (hex.dive > 0? hex.dive : 0);

            //for everyone other than the first column
            if (lastCol != null)
            {
                // if we're over the hump, the lastCol is too high. Take care of it and move lower to be consistent
                if (hex.dive > 0)
                {
                    lastCol.row = hex;
                    hex.lup = lastCol;

                    lastHex = lastCol.dow;
                }

                //else as in hump and leftwards
                else lastHex = lastCol;

				//this should always be the upperRight Hex at this point
                lastHex.rup = hex;
                hex.low = lastHex;
            }

            //track the most recent hex, going down the rows
            Hex lastRow = hex;

            // j is the rise column
            //complicated because j is not a y coordinate but we're treating it kinda like one
            for (int j = hex.rise-1; j >= (Math.Abs(i) - 9) - (i>0?i:0); j--)
            {
                //instantiate next hex down
                Hex x = new Hex();
                x.dive = i;
                x.rise = j;
                x.fall = lastRow.fall - 1;

                //track the center Hex
                if (i == 0 && j == 0)
                {
                    rockStar = x;
                    rockStar.mySpace = GameObject.Instantiate(
                        block,
                        new Vector3(0, 0, 0),
                        Quaternion.identity);
					SpriteRenderer sr = rockStar.mySpace.GetComponent<SpriteRenderer>();
					sr.sprite = tech;
                }

                //set relationship with upper hex
                x.upp = lastRow;
                lastRow.dow = x;
                
                //establish relationships with hexes to the left
                if (lastHex != null)
                {
                    //should always be an upper left hex
                    lastHex.row = x;
                    x.lup = lastHex;

                    //move the left col hex tracking down one 
                    lastHex = lastHex.dow;

                    //lower left possibly
                    if (lastHex != null)
                    {
                        lastHex.rup = x;
                        x.low = lastHex;
                    }
                }

				//track this for the next row
                lastRow = x;
            }

			//track this for the next column
            lastCol = hex;
        }

		//create the starting state
		hire();
	}//end of start


	//creates a new band of blocks in one of the six corners, chosen at random
	//the game will end if we are not able to hire a new band
	void hire() {
		Hex current = null;
		int blocked = 0;
		//bool[] impossible = new bool[] { false, false, false, false, false, false };
		int ind = UnityEngine.Random.Range(0, 6);
		current = getACorner(ind);

		while (blocked < 6) {

			band.Add(current.makeSpace(block));
			for (int i = 0; i < 3; i++) {

				//reuse of current as a Hex placeholder
				current = getRandomHex(current);

				//as long as a valid Hex was retrieved, add it to the band and make exit of loop possible by setting blocked to 6
				if (current != null) {
					band.Add(current.makeSpace(block) );
					blocked = 6;
				}

				//if there was no valid Hex retrieved, we need to count the occurence, and search again with an increased index
				else
				{
					blocked++;
					ind++;
					if (ind > 5) ind = 0;
					current = getACorner(i);
					fireEveryone();
					break; //no point in continuing the for loop
				}
			}
		}

		if (band.Count == 0) gameOver();

		//set the direction and make ghosts for each band member
		//makeghost relies on proper direction being set first
		getDirections();
		foreach (Hex hex in band) makeGhost(hex);

	}


	//clears the band to have nothing in it by destroying the objects and removing the hexes from the list
	void fireEveryone()
    {
		foreach (Hex hex in band)
        {
			GameObject.Destroy(hex.mySpace);
			band.Remove(hex);
        }
    }


	//the show is over, folks
	void gameOver()
    {
		Application.Quit();
    }


	//gets one of the six corners based on a given index
	Hex getACorner(int i)
    {
		Hex corner = null;
		switch (i)
		{
			case 0: corner = getHex(00, 09, false); i = 0; break;
			case 1: corner = getHex(00, -9, false); i = 1; break;
			case 2: corner = getHex(-9, 00, false); i = 2; break;
			case 3: corner = getHex(09, 09, false); i = 3; break;
			case 4: corner = getHex(-9, -9, false); i = 4; break;
			case 5: corner = getHex(09, 00, false); i = 5; break;
		}
		return corner;
	}

    // Update is called once per frame
    void Update(){
		bool collide;
		List<Hex> newBand;

		if (Input.GetKeyUp("right"))
		{	collide = false;

			foreach (Hex hex in band)
			{  //determine if the right hex is upper right or lower
				Hex myRight = getRight(hex);

				//determine if you can move right or not
				if (moveBump(hex, myRight)) collide = true;
			}

			if (!collide)
			{	//bool far = true; //by default we assume the movement will be moving us farther away

				//form the new band
				newBand = new List<Hex>();
				foreach (Hex hex in band)
				{	Hex bumped = getRight(hex);
					newBand.Add(bumped);

					//if at least one hex stays at the same or closer proximity, the move is legal
					//if (prox(bumped) <= prox(hex)) far = false;
				}

				/*
				 * removed feature
				 * 
				//you can move seats as long as you arent moving away from center stage
				if (!far)
				{*/
					bustGhosts();
					foreach (Hex hex in band) GameObject.Destroy(hex.mySpace);
					foreach (Hex hex in newBand)
					{
						hex.makeSpace(block);
						makeGhost(hex);
					}
					band = newBand;
				//}
				
			}
			else Debug.Log("collision");
		}
		else if (Input.GetKeyUp("down"))
		{	collide = false;

			foreach (Hex hex in band)
			{	if (moveBump(hex, hex.dow)) collide = true;}

			if (!collide)
			{
				//bool far = true;
				newBand = new List<Hex>();
				foreach (Hex hex in band)
				{	newBand.Add(hex.dow);
					//if (prox(hex.dow) <= prox(hex)) far = false;
				}

				//if (!far)
				//{	
					bustGhosts();
					foreach (Hex hex in band) GameObject.Destroy(hex.mySpace);
					foreach (Hex hex in newBand)
					{	hex.makeSpace(block);
						makeGhost(hex);
					}
					band = newBand;
				//}
				
			}
		}
		else if (Input.GetKeyUp("up"))
		{	collide = false;

			foreach (Hex hex in band)
			{	if (moveBump(hex, hex.upp)) collide = true;}

			if (!collide)
			{	//bool far = true;
				newBand = new List<Hex>();

				foreach (Hex hex in band)
				{	newBand.Add(hex.upp);
					//if (prox(hex.upp) <= prox(hex)) far = false;
				}

				//if (!far)
				//{	
					bustGhosts();
					foreach (Hex hex in band) GameObject.Destroy(hex.mySpace);
					foreach (Hex hex in newBand)
					{	hex.makeSpace(block);
						makeGhost(hex);
					}
					band = newBand;
				//}
				
			}
		}
		else if (Input.GetKeyUp("left"))
		{	collide = false;

			foreach (Hex hex in band)
			{	Hex myLeft = getLeft(hex);
				if (moveBump(hex, myLeft)) collide = true;
			}

			if (!collide)
			{	//bool far = true;
				newBand = new List<Hex>();
				foreach (Hex hex in band)
				{	Hex bumped = getLeft(hex);
					newBand.Add(bumped);
					//if (prox(bumped) <= prox(hex)) far = false;
				}

				
				//if (!far)
				//{	
					bustGhosts();
					foreach (Hex hex in band) GameObject.Destroy(hex.mySpace);
					foreach (Hex hex in newBand)
					{	hex.makeSpace(block);
						makeGhost(hex);
					}
					band = newBand;
				//}
				
			}
		}

		else if (Input.GetKeyUp(KeyCode.Space)) { countdown = 0; }

		//calculate correct direction
		//needed to be after any movement
		if (Input.GetKeyUp("right") ||
			Input.GetKeyUp("down")  ||
			Input.GetKeyUp("up")    ||
			Input.GetKeyUp("left")  ) {
			
			getDirections();
		}

		//coundown to the next moment to move the blocks
		//countdown--; removed this because it wasn't fun

		//currently, this will exit the function entirely if space bar was not pressed
		if (countdown > 0 || band == null) { return; }

		//This point onwards signifies that spacebar was pressed, which should either move the band or seat them
		
		//we'll make new ghosts later, won't need the current ghosts
		bustGhosts();

		//store the Hexes for the updated position in a new list, which will replace the old one
		newBand = new List<Hex>();

		//for each Hex in the band list, destroy the block, update in the proper direction, and create a new block
		getDirections();

		//now that we know the proper direction
		collide = false; //keeps track if the new direction is valid. It is valid until there is a hex found that isn't part of the band
		foreach (Hex hex in band) {
			if      (dir == "dow") {
				newBand.Add(hex.dow);
				if (hex.dow.mySpace != null && !band.Contains(hex.dow) ) collide = true;
			}
			else if (dir == "upp") {
				newBand.Add(hex.upp);
				if (hex.upp.mySpace != null && !band.Contains(hex.upp) ) collide = true;
			}
			else if (dir == "row") {
				newBand.Add(hex.row);
				if (hex.row.mySpace != null && !band.Contains(hex.row) ) collide = true;
			}
			else if (dir == "lup") {
				newBand.Add(hex.lup);
				if (hex.lup.mySpace != null && !band.Contains(hex.lup) ) collide = true;
			}
			else if (dir == "rup") {
				newBand.Add(hex.rup);
				if (hex.rup.mySpace != null && !band.Contains(hex.rup) ) collide = true;
			}
			else if (dir == "low") {
				newBand.Add(hex.low);
				if (hex.low.mySpace != null && !band.Contains(hex.low) ) collide = true;
			}
		}
			
		//if there was a collision, the band is a-BAND-oned and dis-BAND-ed
		if (collide) {
			//color the blocks so that a player can easily tell what spaces need to be filled
			//the color of a block is the same as all others in its circle
			//this can be determined easily by adding the abslute values of hex dive and rise
			foreach (Hex hex in band) {
				int i = Math.Abs(hex.dive) + Math.Abs(hex.fall) + Math.Abs(hex.rise);
				hex.mySpace.GetComponent<SpriteRenderer>().sprite = wardrobe[(i/2)-1];
				attend[ (i/2) - 1 ].Add(hex);
			}

			//clear a circle if its full
			for (int j = 0; j < 9; j++) {
				Debug.Log("attend[" + j + "] is " + attend[j].Count);
				if (attend[j].Count >= (j+1)*6) {
					foreach (Hex hex in attend[j]) {
						GameObject.Destroy(hex.mySpace);
						hex.mySpace = null;
					}
					attend[j].Clear();
				}
			}
			//clear out the band list, and fill it with new blocks
			band.Clear();
			hire();
		}
		//play at the new gig if there was no collision
		else {
			foreach (Hex hex in    band) GameObject.Destroy(hex.mySpace);

			//do after all the previous blocks have been destroyed
			//to prevent duplicates or destroying newly created blocks
			foreach (Hex hex in newBand) {
				hex.makeSpace(block);
				makeGhost(hex);
			}
			band = newBand;
		}

		countdown = 5000;
    }//end of update

	// makes the shadow that indicates what direction is headed towards
	void makeGhost(Hex hex) {
		if (hex == null) return;
		GameObject x = GameObject.Instantiate(ghostBlock, new Vector3(h * hex.dive, (.64f * hex.fall) - (.32f * hex.dive) , 0), Quaternion.identity);
		switch (dir) {
			case "upp" : x.transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self); break;
			case "rup" : x.transform.Rotate(0.0f, 0.0f, 30.0f, Space.Self); break;
			case "row" : x.transform.Rotate(0.0f, 0.0f,-30.0f, Space.Self); break;
			case "dow" : x.transform.Rotate(0.0f, 0.0f,-90.0f, Space.Self); break;
			case "low" : x.transform.Rotate(0.0f, 0.0f,210.0f, Space.Self); break;
			case "lup" : x.transform.Rotate(0.0f, 0.0f,150.0f, Space.Self); break;
		}
	}

	//destroys every object with a ghost tag
	void bustGhosts() {
		GameObject[] objects = GameObject.FindGameObjectsWithTag("ghost");
		foreach (GameObject o in objects) GameObject.Destroy(o);
	}

	//up and down is never ambiguous so there is no getUp() and getDown()
	//left and right are defined by how close the band is to the center
		//left is down-left when we are on the left side, but on the right side it is up-left
		//right is down-rightt when we are on the left side, but on the right side it is up-right
	Hex getRight(Hex hex) {
		Hex myRight = null;
		if (isDown()) myRight =  getAvgDive() >= 0 ? hex.row : hex.rup;
		else myRight = getAvgDive() >= 0 ? hex.rup : hex.row;

		return myRight;
	}

	//^^
	Hex getLeft(Hex hex) {
		Hex myLeft = null;
		if (isDown()) myLeft = getAvgDive() <= 0 ? hex.low : hex.lup;
		else myLeft = getAvgDive() <= 0 ? hex.lup : hex.low;

		return myLeft;
	}


	// destroys the object sent and removes it from any necessary list
	public void getAutograph(GameObject wall) {
		
		//if the bouncer hits the center rockstar, then destroy all blocks
		if (wall == rockStar.mySpace){
			foreach (List<Hex> list in attend){
				foreach (Hex hex in list){
					if ( !band.Contains(hex) ) {
						GameObject.Destroy(hex.mySpace);
					}
                }
				list.Clear();
            }
			return;
		}
		if (wall.tag != block.tag) return; // don't do anything to the things tagged as block

		// do not destroy anyone in the band
		bool busy = false;
		foreach (Hex hex in band) {
			if (wall == hex.mySpace) busy = true;
		}
		if (busy) return;

		// Do what you came here to do
		GameObject.Destroy(wall);

		
		//remove null hex references (because we DESTROYED them)
		List<Hex> found = null;
		Hex me = null;
		foreach (List<Hex> list in attend) {
			foreach (Hex hex in list) {
				if (hex.mySpace == null) {
					found = list;
					me = hex;
					break;
				}
			}
		}
		if (found != null) found.Remove(me);
	}

	//sets the instance variable dir to either dow, upp, row, lup, rup, or low
	//this is based upon which of the three axis the band is closest to, and falls towards the center
	void getDirections() {
			//add together all the dive, fall, and rise axis from the whole band. The one closest to 0 will determine the direction
			int dive = 0, fall = 0, rise = 0;
			foreach (Hex hex in band) {
				dive += hex.dive;
				fall += hex.fall;
				rise += hex.rise;
			}

			//calculate correct direction
			//there is no else statement because the default behavior is not to change direction
				//that happens when the direction could be either of two, and we just keep the existing one (do nothing)
			if (Math.Abs(dive) < Math.Abs(fall) && Math.Abs(dive) < Math.Abs(rise) ) {
				if (fall > 0) dir = "dow";
				else		  dir = "upp";
			}
			else if (Math.Abs(fall) < Math.Abs(dive) && Math.Abs(fall) < Math.Abs(rise) ) {
				if (dive < 0) dir = "row";
				else		  dir = "lup";
			}
			else if (Math.Abs(rise) < Math.Abs(dive) && Math.Abs(rise) < Math.Abs(fall) ) {
				if (dive < 0) dir = "rup";
				else		  dir = "low";
			}
	}


	//average dive
	//used in knowing if you are over the middle line (dive 0)
	float getAvgDive() {
		float avg = 0;
		foreach (Hex hex in band) avg += hex.dive;
		return avg / band.Count;
	}


	//how far away you are from center stage
	int prox(Hex hex) {
		return Math.Abs(hex.dive) + Math.Abs(hex.fall) + Math.Abs(hex.rise);
	}


	//is it going downwards?
	//returns true if the direction is low, dow, or row
	bool isDown() {
		return dir == "dow" ||
			   dir == "low" ||
			   dir == "row" ?
			   true : false;
	}


	//checks if a move bumps into anything
	//returns true if there is a hex that is not in the band or if the space does not exist
	bool moveBump(Hex hex, Hex seat) {
		bool collide = false;
		if (seat == null || (seat.mySpace != null && !band.Contains(seat) )) collide = true;
		return collide;
	}


	//get a valid hex in a random direction from the given hex
	Hex getRandomHex(Hex hex) {
		//add all valid hexes to a list
		List<Hex> list = new List<Hex>();
		if (hex.upp != null && hex.upp.mySpace == null) list.Add(hex.upp);
		if (hex.rup != null && hex.rup.mySpace == null) list.Add(hex.rup);
		if (hex.row != null && hex.row.mySpace == null) list.Add(hex.row);
		if (hex.dow != null && hex.dow.mySpace == null) list.Add(hex.dow);
		if (hex.low != null && hex.low.mySpace == null) list.Add(hex.low);
		if (hex.lup != null && hex.lup.mySpace == null) list.Add(hex.lup);

		//pull a random hex from the available list
		int i = UnityEngine.Random.Range(0,list.Count);
		if (list.Count > 0) return list[i];
		return null;
	}


	//get x value based on dive - not in use
	int getDive(float x) {
		return 0;
	}


	//get y value based on fall - not in use
	int getFall(float y) {
		return 0;
	}

	
	//get x,y,z value based on dive and fall - not in use
	Vector3 getPos(int dive, int fall) {
		return new Vector3(0,0,0);
	}


    // finds a Hex based on its dive and fall coordinates
    Hex getHex(int dive, int fall, bool b) {
		//start at the center point
        Hex current = rockStar;

		//iterate through the fall coordinate by going up or down
        while (current.fall > fall) current = current.dow;
        while (current.fall < fall) current = current.upp;

		//iterate through the dive coordinate by going upLeft or downRight
        while (current.dive > dive) current = current.lup;
        while (current.dive < dive) current = current.row;

        return current;
    }

	// finds a Hex based on its dive and rise coordinates
    Hex getHex(int dive, bool b, int rise) {
        Hex current = rockStar;

        while (current.rise > rise) current = current.dow;
        while (current.rise < rise) current = current.upp;

        while (current.dive > dive) current = current.low;
        while (current.dive < dive) current = current.rup;

        return current;
    }
    
	// finds a Hex based on its fall and rise coordinates
    Hex getHex(bool b, int fall, int rise) {
        Hex current = rockStar;

        while (current.fall > fall) current = current.low;
        while (current.fall < fall) current = current.rup;

        while (current.rise > rise) current = current.row;
        while (current.rise < rise) current = current.lup;

        return current;
    }
}


//Very Important class
//Represents a board position, whether a piece resides there or not
//Behaves like a node in a linked list
class Hex
{
	static float h = .55426f; //same as MainController.h
	//declare the coordinate members
	//they can only be set properly at run time
    public int dive;
    public int fall;
    public int rise;

	//declare relationship hexes
	//can only be set properly at run time
    public Hex upp = null;
    public Hex rup = null;
    public Hex row = null;
    public Hex dow = null;
    public Hex low = null;
    public Hex lup = null;

	//the Block, if any, that is tied to this space
    public GameObject mySpace;


	//instantiates a Block Game Object right where this hex is
	public Hex makeSpace(GameObject hexblock) {
		mySpace = GameObject.Instantiate(hexblock, new Vector3(h * dive, (.64f * fall) - (.32f * dive) , 0), Quaternion.identity);
		return this;
	}

	public Hex getNextHex(String dir) {
		switch (dir) {
			case "upp" : return upp;
			case "rup" : return rup;
			case "row" : return row;
			case "dow" : return dow;
			case "low" : return low;
			case "lup" : return lup;
		}
		return null;
	}
}
