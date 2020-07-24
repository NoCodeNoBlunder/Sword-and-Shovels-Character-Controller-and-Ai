
#region Explanation
// This is a Custom class. It will contain the information about the Attack itself
// Whenever an Attack is issued a new Instance of this Object "Attack" will be created containing information about how much dmg is being applied and if
// the attack was a crit or not! (for effects)
#endregion

#region Eigene Kommentar: Custom Classe vs Scriptable Object
// Das ist ein guted Beispiel um so sehen wann man ein Custom Classe benutzen sollte und wann man ein Scriptable Object nutzten sollte.
// Eine neue Instanz einer Custom Klasse kann ganz einfach im Code erstellt werden. (Bsp. Attack attack1 = new Attack(coreDamage, isCrit)
// Anders hingegen ein Scriptable Object das nur per Inspector erstellt werden kann und für Objekt genutzt wird die einmalig definiert werden 
// Bevor das Spiel läuft und dann als Ressourcen verfügbar sind. In anderen Worten eine neue Instant von einer Custom Klasse kann erstellt werden wenn
// das Programm läuft, während eine neue Instanz eines SO nur während des development per Hand erstellt werden kann.
#endregion

public class Attack
{
    #region Komment Keyword: readonly
    // readonly indicates that assignment to the field can only be made as part of the declaration or in a Construtor in the same class.#
    #endregion
    private readonly int damage;
    private bool isCritical;

    public Attack(int damage, bool isCritical)
    {
        this.damage = damage;
        this.isCritical = isCritical;
    }

    public int Damage
    {
        get { return damage; }
    }

    // We want to access the Attacks damage and if it was critical
    // So we use a accessor to to let other Script to access these values.
    public bool IsCritical
    {
        get { return isCritical; }
    }
}
