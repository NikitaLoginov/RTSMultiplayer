using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject healthBarParent;
    [SerializeField] private Image healthBarImage;


    public void OnPointerEnter(PointerEventData eventData) => healthBarParent.SetActive(true);

    public void OnPointerExit(PointerEventData eventData) => healthBarParent.SetActive(false);

    private void Awake() => health.ClientOnHealthUpdated += HandleHealthUpdated;

    private void OnDestroy() => health.ClientOnHealthUpdated -= HandleHealthUpdated;

    private void HandleHealthUpdated(int currentHealth, int maxHealth) 
        => healthBarImage.fillAmount = (float)currentHealth / maxHealth;
}
