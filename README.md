# Dokumentation: Wie man Rechtecke aus Punkten findet

Dieses Dokument erklärt Schritt für Schritt einen Algorithmus. Der Algorithmus findet alle Rechtecke in einer Liste von Punkten.

## Eingabedaten (Beispiel)

Wir verwenden diese 19 Punkte als Beispiel für alle Schritte.

```csharp
public static List<Point> POINTS = new List<Point>
{
    new Point(1, 1), new Point(1, 2), new Point(2, 0), new Point(2, 1),
    new Point(2, 4), new Point(2, 5), new Point(2, 6), new Point(2, 7),
    new Point(3, 1), new Point(3, 2), new Point(4, 8), new Point(5, 0),
    new Point(5, 4), new Point(6, 2), new Point(6, 3), new Point(7, 2),
    new Point(7, 3), new Point(9, 1), new Point(9, 7)
};
```
<img width="2098" height="1394" alt="Rectangle-Points" src="https://github.com/user-attachments/assets/62d6d940-0261-4a47-b372-d2fd91b12388" />
https://www.geogebra.org/graphing/jn7vasfy

-----

## Schritt 1: Daten bereinigen

### Erklärung

Zuerst räumen wir die Daten auf. Wir entfernen alle doppelten Punkte. Das ist wichtig, damit der Algorithmus schnell und korrekt arbeitet.

### Code

Das machen wir am Anfang mit der Methode `Distinct()`.

```csharp
// Aus der Datei: Program.cs
var allDistinctPoints = allPoints.Distinct().ToList();
DataProcessor.ProcessData(allDistinctPoints);
```

-----

## Schritt 2: Punkte nach Y-Achse gruppieren

### Erklärung

Ein Rechteck hat horizontale Seiten. Die Punkte auf diesen Seiten haben denselben Y-Wert. Deshalb gruppieren wir alle Punkte nach ihrem Y-Wert. So finden wir schnell alle Punkte, die auf einer horizontalen Ebene liegen.

### Code

Die Methode `GroupPointsByY` erledigt das. Sie erstellt Gruppen von Punkten. Jede Gruppe enthält Punkte mit dem gleichen Y-Wert.

```csharp
// Aus der Datei: RectangleProcessorParallel.cs
public Dictionary<int, List<Point>> GroupPointsByY(List<Point> points)
{
    var pointsGroupedByY = new Dictionary<int, List<Point>>();
    foreach (var point in points)
    {
        if (!pointsGroupedByY.TryGetValue(point.Y, out var group))
        {
            group = new List<Point>();
            pointsGroupedByY[point.Y] = group;
        }
        group.Add(point);
    }
    return pointsGroupedByY;
}
```

### Beispiel

Mit unseren Daten entstehen diese Gruppen:

  - **Y = 0:** `(2, 0), (5, 0)`
  - **Y = 1:** `(1, 1), (2, 1), (3, 1), (9, 1)`
  - **Y = 2:** `(1, 2), (3, 2), (6, 2), (7, 2)`
  - **Y = 3:** `(6, 3), (7, 3)`
  - **Y = 4:** `(2, 4), (5, 4)`
  - **Y = 7:** `(2, 7), (9, 7)`

-----

## Schritt 3: Horizontale Linien erstellen

### Erklärung

Jetzt erstellen wir aus den Gruppen horizontale Linien. Eine Linie braucht mindestens zwei Punkte. Diese Linien sind die möglichen Ober- und Unterseiten unserer Rechtecke. Um das schnell zu machen, läuft dieser Prozess parallel.

### Code

Die Methode `CreateLines` nimmt die Gruppen und erstellt Linien. `Parallel.ForEach` beschleunigt den Prozess.

```csharp
// Aus der Datei: RectangleProcessorParallel.cs
public List<Line> CreateLines(Dictionary<int, List<Point>> pointsGroupedByY)
{
    var lines = new ConcurrentBag<Line>();
    Parallel.ForEach(pointsGroupedByY.Values, group =>
    {
        if (group.Count < 2) return; // Man braucht mindestens 2 Punkte

        for (int i = 0; i < group.Count; i++)
        {
            for (int j = i + 1; j < group.Count; j++)
            {
                lines.Add(new Line(group[i], group[j]));
            }
        }
    });
    return lines.ToList();
}
```

-----

## Schritt 4: Rechtecke finden

### Erklärung

Das ist der Kern des Algorithmus. Wir nehmen jede Linie als **"Basis-Linie"**. Dann suchen wir eine zweite, passende Linie.

Eine passende Linie muss zwei Regeln erfüllen:

1.  Sie muss einen **anderen Y-Wert** haben.
2.  Sie muss die **gleichen X-Werte** haben wie die Basis-Linie.

Wenn wir so eine Linie finden, haben wir ein Rechteck gefunden.

### Code

Die Methode `FindPotentialRectangles` setzt diese Logik um.

```csharp
// Aus der Datei: RectangleProcessorParallel.cs
public List<IRectangle> FindPotentialRectangles(List<Line> lines)
{
    var potentialRectangles = new ConcurrentDictionary<IRectangle, bool>();
    // ... Gruppierung der Linien nach Y ...

    Parallel.ForEach(linesGroupedByY, baseGroup =>
    {
        foreach (var baseLine in baseGroup.Value)
        {
            foreach (var comparisonGroup in linesGroupedByY)
            {
                if (comparisonGroup.Key <= baseGroup.Key) continue;

                foreach (var comparisonLine in comparisonGroup.Value)
                {
                    // Prüfen, ob die X-Werte gleich sind
                    if ((baseLine.Point1.X == comparisonLine.Point1.X && baseLine.Point2.X == comparisonLine.Point2.X) ||
                        (baseLine.Point1.X == comparisonLine.Point2.X && baseLine.Point2.X == comparisonLine.Point1.X))
                    {
                        var newRectangle = new Models.Rectangle(baseLine, comparisonLine);
                        potentialRectangles.TryAdd(newRectangle, true);
                    }
                }
            }
        }
    });
    return potentialRectangles.Keys.ToList();
}
```

### Beispiel

1.  Der Algorithmus wählt die **Basis-Linie** von `(2,0)` bis `(5,0)`. Die X-Werte sind 2 und 5.
2.  Er sucht in anderen Y-Ebenen eine Linie mit den X-Werten 2 und 5.
3.  Er findet die Linie von `(2,4)` bis `(5,4)`.
4.  **Ergebnis:** Ein Rechteck wurde gefunden\!

-----

## Schritt 5: Duplikate vermeiden

### Erklärung

Ein Rechteck kann mehrfach gefunden werden. Wir müssen das verhindern. Dafür benutzen wir ein `ConcurrentDictionary`. Es speichert jedes gefundene Rechteck. Wenn ein Rechteck schon existiert, wird es nicht nochmal hinzugefügt. Das funktioniert, weil wir eine spezielle `Equals`-Methode für unsere Rechtecke geschrieben haben.

### Code

Diese `Equals`-Methode in der `Rectangle`-Klasse ist sehr wichtig. Sie prüft, ob zwei Rechtecke dieselben vier Eckpunkte haben.

```csharp
// Aus der Datei: Rectangle.cs
public bool Equals(IRectangle? other)
{
    if (other == null) return false;
    
    // Zwei Rechtecke sind gleich, wenn ihre 4 Eckpunkte gleich sind.
    return GetOrderedPoints().SequenceEqual(other.GetOrderedPoints());
}
```

-----

## Schritt 6: Das Endergebnis

### Erklärung

Am Ende gibt der Algorithmus eine saubere Liste zurück. In dieser Liste ist jedes gefundene Rechteck nur einmal enthalten.

### Ergebnis für unser Beispiel

Der Algorithmus findet diese **4 einzigartigen Rechtecke**:

1.  **Rechteck 1:** Ecken bei (1,1), (3,1), (1,2), (3,2)
2.  **Rechteck 2:** Ecken bei (2,0), (5,0), (2,4), (5,4)
3.  **Rechteck 3:** Ecken bei (6,2), (7,2), (6,3), (7,3)
4.  **Rechteck 4:** Ecken bei (2,1), (9,1), (2,7), (9,7)

-----

## Code-Details: Ein genauerer Blick auf C\#

In diesem Teil schauen wir uns den C\#-Code genauer an. Wir erklären wichtige Methoden und Befehle so einfach wie möglich. Das hilft jedem zu verstehen, wie der Code funktioniert, auch ohne tiefe C\#-Kenntnisse.

### Die Methode `GroupPointsByY`

Diese Methode ist der erste Schritt im Algorithmus. Sie nimmt eine Liste von Punkten und sortiert sie in Gruppen. Jeder Punkt kommt in eine Gruppe, die zu seinem Y-Wert gehört.

**Der Code:**

```csharp
public Dictionary<int, List<Point>> GroupPointsByY(List<Point> points)
{
    var pointsGroupedByY = new Dictionary<int, List<Point>>();
    foreach (var point in points)
    {
        if (!pointsGroupedByY.TryGetValue(point.Y, out var group))
        {
            group = new List<Point>();
            pointsGroupedByY[point.Y] = group;
        }
        group.Add(point);
    }
    return pointsGroupedByY;
}
```

-----

#### Was bedeuten die Befehle?

**1. `Dictionary<int, List<Point>>`**

  * Ein `Dictionary` ist eine Art Sammlung zum Speichern von Datenpaaren. Es ist wie ein Telefonbuch.
  * Jedes Paar besteht aus einem **Schlüssel (Key)** und einem **Wert (Value)**. Der Schlüssel muss einzigartig sein.
  * In unserem Code ist der Schlüssel eine `int` (Ganzzahl). Das ist der **Y-Wert**.
  * Der Wert ist eine `List<Point>` (eine Liste von Punkten). Das sind **alle Punkte** mit diesem Y-Wert.
  * `var pointsGroupedByY = new Dictionary...` erstellt ein neues, leeres Dictionary.

**2. `foreach (var point in points)`**

  * Das ist eine Schleife. Sie geht die Liste `points` durch, Punkt für Punkt.
  * Bei jeder Wiederholung der Schleife enthält die Variable `point` den aktuellen Punkt.

**3. `pointsGroupedByY.TryGetValue(point.Y, out var group)`**

  * Das ist der cleverste Teil dieser Methode. Lassen Sie es uns langsam erklären.
  * **`TryGetValue`** bedeutet: "Versuche, den Wert zu bekommen".
  * Der Befehl versucht, den Schlüssel `point.Y` (den Y-Wert des aktuellen Punktes) im Dictionary zu finden.

Es gibt **zwei mögliche Ergebnisse**:

  * **Fall A: Der Schlüssel existiert bereits.**

      * Das bedeutet, wir hatten schon einen Punkt mit diesem Y-Wert.
      * `TryGetValue` findet die passende Liste von Punkten und speichert sie in der Variable `group`.
      * Der Befehl gibt `true` (wahr) zurück.
      * Wegen dem `!` (Nicht-Operator) wird `!true` zu `false`. Die `if`-Bedingung ist also nicht erfüllt und der Code im `if`-Block wird **übersprungen**.

  * **Fall B: Der Schlüssel existiert noch nicht.**

      * Das bedeutet, dies ist der erste Punkt, den wir auf dieser Y-Ebene sehen.
      * `TryGetValue` findet nichts. Die Variable `group` bleibt leer.
      * Der Befehl gibt `false` (falsch) zurück.
      * Wegen dem `!` (Nicht-Operator) wird `!false` zu `true`. Die `if`-Bedingung ist erfüllt und der Code im `if`-Block wird **ausgeführt**.

**4. Der `if`-Block**

  * Dieser Code wird nur ausgeführt, wenn eine Y-Ebene zum ersten Mal auftaucht.
      * `group = new List<Point>();` erstellt eine **neue, leere Liste** für die Punkte.
      * `pointsGroupedByY[point.Y] = group;` fügt das neue Paar zum Dictionary hinzu. Der Y-Wert ist der Schlüssel und die neue, leere Liste ist der Wert.

**5. `group.Add(point)`**

  * Dieser Befehl steht außerhalb des `if`-Blocks. Er wird also **immer** ausgeführt.
  * Er fügt den aktuellen `point` zur richtigen Gruppe (`group`) hinzu.
  * Wenn die Gruppe neu war, ist es jetzt der erste Punkt in dieser Liste.
  * Wenn die Gruppe schon existierte, wird der Punkt einfach zur bestehenden Liste hinzugefügt.

Am Ende gibt die Methode das fertige Dictionary zurück, in dem alle Punkte ordentlich nach ihrem Y-Wert gruppiert sind.

-----

### Die Methode `CreateLines`

Nachdem wir die Punkte gruppiert haben, ist der nächste Schritt, aus ihnen Linien zu machen. Diese Methode nimmt die Gruppen von Punkten und erstellt alle möglichen horizontalen Linien innerhalb jeder Gruppe.

**Der Code:**

```csharp
public List<Line> CreateLines(Dictionary<int, List<Point>> pointsGroupedByY)
{
    var lines = new ConcurrentBag<Line>();

    Parallel.ForEach(pointsGroupedByY.Values, group =>
    {
        // Eine Gruppe muss mindestens zwei Punkte haben, um eine Linie zu bilden.
        if (group.Count < 2) return;

        for (int i = 0; i < group.Count; i++)
        {
            for (int j = i + 1; j < group.Count; j++)
            {
                var newLine = new Line(group[i], group[j]);
                lines.Add(newLine);
            }
        }
    });

    return lines.ToList();
}
```

-----

#### Was bedeuten die Befehle?

**1. `ConcurrentBag<Line>`**

  * Stellen Sie sich einen normalen "Beutel" (`Bag`) vor, in den Sie Dinge hineinwerfen können. In C\# ist eine normale Liste (`List`) wie ein ordentlicher Stapel. Ein `Bag` ist eher wie ein Sack, die Reihenfolge ist nicht so wichtig.
  * Das Wort **`Concurrent`** bedeutet "gleichzeitig".
  * Ein `ConcurrentBag` ist also ein spezieller "Beutel", der **sicher von mehreren Arbeitern gleichzeitig benutzt werden kann**. In der Programmierung nennen wir diese Arbeiter **Threads**.
  * Wenn mehrere Threads gleichzeitig versuchen, etwas in eine normale Liste zu legen, kann es zu Fehlern oder Datenverlust kommen. Ein `ConcurrentBag` ist dafür gebaut, das zu verhindern. Er ist **thread-sicher**.
  * `var lines = new ConcurrentBag<Line>();` erstellt einen neuen, leeren und thread-sicheren Beutel, in den wir unsere `Line`-Objekte legen werden.

**2. `Parallel.ForEach`**

  * Ein normales `foreach` ist wie ein einzelner Arbeiter, der eine Liste von Aufgaben eine nach der anderen abarbeitet.
  * **`Parallel.ForEach`** ist wie ein **Team von Arbeitern**. Es teilt die Aufgabenliste unter den Arbeitern auf, und alle arbeiten **gleichzeitig** an ihren Aufgaben.
  * In einem modernen Computer hat der Prozessor (CPU) mehrere Kerne (`Cores`). Jeder Kern kann wie ein eigener Arbeiter sein. `Parallel.ForEach` nutzt diese Kerne, um die Arbeit viel schneller zu erledigen.

**Wie funktioniert es hier?**

1.  `Parallel.ForEach(pointsGroupedByY.Values, ...)` bekommt die Liste aller unserer Punkt-Gruppen.
2.  Anstatt die Gruppen nacheinander zu verarbeiten, teilt es die Gruppen auf die verfügbaren CPU-Kerne auf.
3.  **Zum Beispiel:**
      * Kern 1 bekommt die Gruppe `Y = 1` und erstellt alle Linien daraus.
      * Kern 2 bekommt die Gruppe `Y = 2` und erstellt *gleichzeitig* alle Linien daraus.
      * Kern 3 bekommt die Gruppe `Y = 4` und arbeitet *auch gleichzeitig*.
4.  Der Code innerhalb der Schleife (`for (int i...`) ist der gleiche wie bei einer normalen Schleife. Er findet alle Paare von Punkten und erstellt eine `Line`.
5.  `lines.Add(newLine);` wirft die neu erstellte Linie in den `ConcurrentBag`. Da der Beutel thread-sicher ist, gibt es kein Problem, dass mehrere Kerne gleichzeitig Linien hinzufügen.

Am Ende, wenn alle Teams ihre Arbeit erledigt haben, wandelt `lines.ToList()` den Beutel wieder in eine normale Liste um und gibt sie zurück.

**Zusammenfassend:** Diese Methode nutzt moderne Hardware voll aus, indem sie die Arbeit des Linien-Erstellens auf mehrere Schultern verteilt und so den gesamten Prozess erheblich beschleunigt. 🚀

-----

### Die Methode `FindPotentialRectangles`

Das ist der wichtigste Teil des Programms. Hier passiert die eigentliche "Magie". Die Methode nimmt die Liste aller horizontalen Linien und findet heraus, welche davon Rechtecke bilden.

**Der Code:**

```csharp
public List<IRectangle> FindPotentialRectangles(List<Line> lines)
{
    var potentialRectangles = new ConcurrentDictionary<IRectangle, bool>();

    var linesGroupedByY = lines.GroupBy(line => line.Point1.Y)
                               .ToDictionary(g => g.Key, g => g.ToList());

    Parallel.ForEach(linesGroupedByY, baseGroup =>
    {
        foreach (var baseLine in baseGroup.Value)
        {
            foreach (var comparisonGroup in linesGroupedByY)
            {
                // **Bedingung 1**
                if (comparisonGroup.Key <= baseGroup.Key)
                {
                    continue;
                }

                foreach (var comparisonLine in comparisonGroup.Value)
                {
                    // **Bedingung 2**
                    if ((baseLine.Point1.X == comparisonLine.Point1.X && baseLine.Point2.X == comparisonLine.Point2.X) ||
                        (baseLine.Point1.X == comparisonLine.Point2.X && baseLine.Point2.X == comparisonLine.Point1.X))
                    {
                        var newRectangle = new Models.Rectangle(baseLine, comparisonLine);
                        potentialRectangles.TryAdd(newRectangle, true);
                    }
                }
            }
        }
    });

    return potentialRectangles.Keys.ToList();
}
```

-----

#### Was bedeuten die Befehle?

#### 1\. `ConcurrentDictionary<IRectangle, bool>`**

  * Wie wir schon wissen, ist `Concurrent` für die Arbeit im Team (parallel).
  * Ein `Dictionary` speichert Datenpaare. Hier ist der Schlüssel (`Key`) das `IRectangle` (unser Rechteck) und der Wert (`Value`) ein einfacher `bool`-Wert (`true` oder `false`).
  * Wir benutzen es hier wie einen Türsteher. Es lässt jedes Rechteck nur **einmal** hinein. Wenn ein Rechteck schon drin ist, wird es ignoriert. Das verhindert Duplikate.
  * Der `bool`-Wert selbst ist hier nicht wichtig. Wir nutzen nur die Eigenschaft des Dictionarys, dass jeder Schlüssel einzigartig sein muss.

-----


#### 2\. Linien neu gruppieren: `lines.GroupBy(...)`

Ganz am Anfang der Methode sehen wir diese Codezeile:

```csharp
var linesGroupedByY = lines.GroupBy(line => line.Point1.Y)
                           .ToDictionary(group => group.Key, group => group.ToList());
```

**Was passiert hier? Schritt für Schritt:**

1.  **`lines.GroupBy(line => line.Point1.Y)`**:

      * **Problem:** Wir haben eine lange Liste mit allen möglichen Linien. Diese ist unsortiert.
      * **Befehl:** `GroupBy` ist ein sehr nützlicher Befehl in C\#. Er funktioniert wie eine Sortiermaschine. Er nimmt die lange Liste und steckt jede Linie in einen "Eimer".
      * **Regel:** Die Regel zum Sortieren ist `line => line.Point1.Y`. Das bedeutet: "Nimm den Y-Wert des ersten Punktes einer Linie als Etikett für den Eimer."
      * **Ergebnis:** Wir haben jetzt mehrere Eimer. Jeder Eimer hat ein Y-Etikett (z.B. `Y=1`, `Y=2`) und enthält alle Linien, die auf dieser Y-Ebene liegen.

2.  **`.ToDictionary(...)`**:

      * Diese "Eimer" von `GroupBy` sind nur temporär. Der Befehl `.ToDictionary()` macht daraus eine permanente, feste Struktur.
      * Er erstellt ein `Dictionary`, das wir schon kennen.
      * `group => group.Key` sagt: "Nimm das Etikett des Eimers (den Y-Wert) als **Schlüssel**."
      * `group => group.ToList()` sagt: "Nimm alle Linien im Eimer und mache daraus eine Liste. Das ist der **Wert**."

**Beispiel:**

  * **Vorher:** `[Line(1,1)-(2,1), Line(6,3)-(7,3), Line(1,1)-(3,1), ...]` (eine lange, unsortierte Liste)
  * **Nachher:** Ein `Dictionary` das so aussieht:
      * **Key: 1** -\> **Value:** `[Line(1,1)-(2,1), Line(1,1)-(3,1), ...]` (alle Linien von Ebene Y=1)
      * **Key: 2** -\> **Value:** `[Line(1,2)-(3,2), Line(6,2)-(7,2), ...]` (alle Linien von Ebene Y=2)
      * **Key: 3** -\> **Value:** `[Line(6,3)-(7,3)]` (alle Linien von Ebene Y=3)
      * ... und so weiter.

Jetzt ist alles sauber organisiert für die große Suche.

-----

#### 3\. Die X-Werte vergleichen: Die `if`-Bedingung

Das ist die entscheidende Prüfung, um ein Rechteck zu bestätigen.

```csharp
if ((baseLineX1 == comparisonLineX1 && baseLineX2 == comparisonLineX2) ||
    (baseLineX1 == comparisonLineX2 && baseLineX2 == comparisonLineX1))
```

**Warum ist diese Bedingung so aufgebaut?**

  * **Das Problem:** Ein `Line`-Objekt hat `Point1` und `Point2`. Aber wir wissen nicht, welcher Punkt links und welcher rechts ist. Eine Linie von `A` nach `B` ist dieselbe Linie wie von `B` nach `A`. Unsere Prüfung muss also beide Möglichkeiten abdecken.
  * Die Bedingung besteht aus **zwei Teilen**, die mit einem **`||` (ODER)** verbunden sind. Wenn einer der beiden Teile `wahr` ist, ist die ganze Bedingung `wahr`.

**Beispiel Schritt für Schritt:**

Nehmen wir an, wir suchen ein Rechteck und haben diese beiden Linien:

  * `baseLine` (auf Y=1): Die Punkte sind `(2,1)` und `(9,1)`.
      * `baseLineX1` = **2**
      * `baseLineX2` = **9**
  * `comparisonLine` (auf Y=7): Die Punkte sind `(9,7)` und `(2,7)`.
      * `comparisonLineX1` = **9**
      * `comparisonLineX2` = **2**

Jetzt prüfen wir die Bedingung:

1.  **Erster Teil:** `(baseLineX1 == comparisonLineX1 && baseLineX2 == comparisonLineX2)`

      * `(2 == 9 && 9 == 2)`
      * `false && false` ergibt **`false`**.

2.  **Zweiter Teil:** `(baseLineX1 == comparisonLineX2 && baseLineX2 == comparisonLineX1)`

      * `(2 == 2 && 9 == 9)`
      * `true && true` ergibt **`true`**.

3.  **Gesamtergebnis:** `false || true` ergibt **`true`**.

      * Perfekt\! Die Linien sind vertikal perfekt ausgerichtet. Ein Rechteck wird erstellt. 🎉

Diese doppelte Prüfung stellt sicher, dass wir kein Rechteck verpassen, nur weil die Punkte in einer Linie "falsch herum" gespeichert sind.

-----

#### 4\. Die Methode `GetMatchingXGroups`

Diese Methode ist einer der intelligentesten Tricks im gesamten Algorithmus. Sie wird sowohl vom sequentiellen als auch vom parallelen Prozessor aufgerufen, um die Suche nach Rechtecken drastisch zu **beschleunigen**. Man kann sie sich als einen sehr effizienten **Vor-Filter** vorstellen.

### Was ist das Ziel dieser Methode?

Stellen Sie sich vor, Sie haben eine **Basis-Linie** und müssen eine passende Partner-Linie auf einer anderen Y-Ebene finden.

  * **Der langsame Weg:** Alle anderen Linien im gesamten Datensatz eine nach der anderen zu überprüfen.
  * **Der schnelle Weg (mit dieser Methode):** Zuerst zu fragen: "In welchen Y-Gruppen gibt es **überhaupt** eine Linie, die als Partner in Frage kommt?"

Diese Methode beantwortet genau diese Frage. Sie gibt eine kleine Liste von vielversprechenden Gruppen zurück, sodass wir unsere Suche auf diese wenigen Gruppen beschränken können.

### Der Code

```csharp
public List<KeyValuePair<int, List<Line>>> GetMatchingXGroups(
    Dictionary<int, List<Line>> linesGroupedByY, int baseLineX1, int baseLineX2)
{
    // Eine leere Liste für die vielversprechenden Gruppen
    var matchingXGroups = new List<KeyValuePair<int, List<Line>>>();

    // Gehe jede Y-Gruppe durch
    foreach (var yGroup in linesGroupedByY)
    {
        // Gehe jede Linie innerhalb dieser Gruppe durch
        foreach (var line in yGroup.Value)
        {
            // Prüfe, ob die X-Werte dieser Linie mit denen der Basis-Linie übereinstimmen
            if ((line.Point1.X == baseLineX1 && line.Point2.X == baseLineX2) ||
                (line.Point1.X == baseLineX2 && line.Point2.X == baseLineX1))
            {
                // Treffer! Füge die gesamte Gruppe zur Ergebnisliste hinzu
                matchingXGroups.Add(yGroup);
                
                // Extrem wichtige Optimierung: Beende die innere Schleife sofort!
                break; 
            }
        }
    }

    return matchingXGroups; // Gib die Liste der relevanten Gruppen zurück
}
```

-----

### Wie die Methode funktioniert (Schritt-für-Schritt-Beispiel)

Nehmen wir an, der Haupt-Algorithmus hat gerade diese **Basis-Linie** ausgewählt:

  * `baseLine` ist die Linie von `(6,2)` bis `(7,2)`.
  * Die gesuchten X-Werte sind also `baseLineX1 = 6` und `baseLineX2 = 7`.

Jetzt ruft der Algorithmus `GetMatchingXGroups(..., 6, 7)` auf.

1.  **Schleife startet:** Die Methode nimmt die erste Gruppe, z.B. von der Ebene `Y=0`.
2.  **Prüfung:** Sie schaut sich alle Linien in der Gruppe `Y=0` an. Gibt es hier eine Linie mit den X-Werten 6 und 7? **Nein.** Es passiert nichts.
3.  **Nächste Gruppe:** Die Methode geht weiter zur Gruppe `Y=1`. Gibt es hier eine passende Linie? **Nein.**
4.  **Der Treffer:** Die Methode erreicht die Gruppe der Ebene **`Y=3`**.
      * Die innere Schleife startet und findet die Linie von `(6,3)` bis `(7,3)`.
      * Die `if`-Bedingung prüft: Hat diese Linie die X-Werte 6 und 7? **Ja\!**
      * **Aktion 1:** `matchingXGroups.Add(yGroup);` -\> Die **gesamte Gruppe** von `Y=3` wird zur Ergebnisliste `matchingXGroups` hinzugefügt.
      * **Aktion 2:** `break;` -\> Dieser Befehl ist der Schlüssel\! Er sagt: "Wir haben einen Treffer in dieser Gruppe gefunden. Es ist nicht nötig, die restlichen Linien in der `Y=3` Gruppe zu prüfen. Wir wissen bereits, dass diese Gruppe wichtig ist." Die innere Schleife wird sofort beendet, was enorm viel Zeit spart.

**Das Endergebnis der Methode:**

Die Methode gibt eine Liste zurück, die **nur die relevanten Gruppen** enthält. In unserem Beispiel wäre das eine Liste, die nur das `KeyValuePair` für die Ebene `Y=3` enthält.

Der Haupt-Algorithmus in `FindPotentialRectangles` muss jetzt nicht mehr alle 5 oder 10 Y-Gruppen durchsuchen. Er durchsucht **nur noch die eine Gruppe**, die ihm diese Methode als vielversprechend geliefert hat. Dies reduziert die Anzahl der Vergleiche dramatisch und ist der Hauptgrund für die hohe Geschwindigkeit des Programms.
