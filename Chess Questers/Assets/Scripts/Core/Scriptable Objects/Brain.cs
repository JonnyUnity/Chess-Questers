using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    public abstract class Brain : ScriptableObject
    {

        public CreatureRuntimeSet PlayerCharacters;
        public CreatureRuntimeSet Enemies;

        public abstract ActionResult GetAction(Enemy enemy);

    }
}