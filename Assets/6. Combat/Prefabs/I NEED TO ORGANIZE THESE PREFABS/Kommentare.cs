using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kommentare : MonoBehaviour { }

/* Man muss differenzieren zischen den Monobehaviouren die man jedes mal neu attachen muss wenn man ein neuen Character erstellt 
 * und nur diesen MonoBehaviouren, die bereits attached sind and die Prefabs die dann nur eingeführt werden müssen!
 * 
 * We are changing the layer of our Enemies and Player since we want the enemies to be clickabkle and this dependce in which 
 * layer they are as the MouseManager is only sending out rays to certain layers! We only want to adjust the layers of the "main" parent #
 * as this is the only part of the golbin we want to click on and not its child objects!
 * 
 * Damit ein Ray ein gameObject treffen kann muss es einen Collider haben ich Idiot, deswegen detected unser ray den Batgoblin nicht und der
 * Cursor ändert sich nicht weshalb wir ihn nicht angreifen könenn! Da wir dem Badgoblin ein Behaviour mit der Interface Attackable hinzugefügt haben 
 * müssen wir nichts weiter machen als dem Batgoblin einen Collider zu geben im MouseManager müssen wir nichts mehr machen!
 * 
 * 
 * Man muss unterscheiden zwsichen verschiedenen Scripten. Manche Scripte sind Interfaces, Container. Spawner etc etc etc!
 * Manager gibt es nur eine Instance oder mehrere Instancen von diesen Objekten. Man muss immer genau die Aufgabe eines jeden Scripted verstehen
 * und am Besten the "SIngle Responsibility Principal" einhalten!
 */

