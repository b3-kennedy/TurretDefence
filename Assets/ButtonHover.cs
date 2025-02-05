using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = true;
    }
}
