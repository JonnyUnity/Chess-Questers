using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace JFlex.ChessQuesters.Encounters.Battle.UI
{
    public class CreaturePortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] private Color _friendlyColour;
        [SerializeField] private Color _enemyColour;
        [SerializeField] private Image _background;
        [SerializeField] private Image _portrait;

        public int ID { get; private set; }
        private Creature _creature;


        public void OnPointerEnter(PointerEventData eventData)
        {
            BattleEvents.CreatureHovered(_creature);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            BattleEvents.CreatureUnhovered(_creature);
        }


        public void SetPortrait(Creature creature)
        {
            _creature = creature;
            ID = creature.ID;
            _background.color = creature.IsFriendly ? _friendlyColour : _enemyColour;
            _portrait.sprite = creature.PortraitSprite;
        }

    }
}