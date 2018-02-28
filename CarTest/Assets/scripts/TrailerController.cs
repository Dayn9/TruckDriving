using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerController : TruckController
{

    //Empty Method Overrides 
    public override GameObject RemoveTrailer() { return null; }    
    public override void OnTriggerEnter(Collider coll) { } //doesn't add trailer to back when it runs into a powerup
    public override void BackToStart() { }
}
