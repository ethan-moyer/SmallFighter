using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitData
{
    public ActionData action { get; private set; }
    public NewCollisionBox hitbox { get; private set; }
    public int hitStun { get; private set; }
    public int blockStun { get; private set; }

    public HitData(ActionData action, NewCollisionBox hitbox, int hitStun, int blockStun)
    {
        this.action = action;
        this.hitbox = hitbox;
        this.hitStun = hitStun;
        this.blockStun = blockStun;
    }
}
