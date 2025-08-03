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

<!-- TOC start (generated with https://github.com/derlin/bitdowntoc) -->

- [Dokumentation: Wie man Rechtecke aus Punkten findet](#dokumentation-wie-man-rechtecke-aus-punkten-findet)
   * [Eingabedaten (Beispiel)](#eingabedaten-beispiel)
   * [Schritt 1: Daten bereinigen](#schritt-1-daten-bereinigen)
      + [Erklärung](#erklärung)
      + [Code](#code)
   * [Schritt 2: Punkte nach Y-Achse gruppieren](#schritt-2-punkte-nach-y-achse-gruppieren)
      + [Erklärung](#erklärung-1)
      + [Code](#code-1)
      + [Beispiel](#beispiel)
   * [Schritt 3: Horizontale Linien erstellen](#schritt-3-horizontale-linien-erstellen)
      + [Erklärung](#erklärung-2)
      + [Code](#code-2)
   * [Schritt 4: Rechtecke finden](#schritt-4-rechtecke-finden)
      + [Erklärung](#erklärung-3)
      + [Code](#code-3)
      + [Beispiel](#beispiel-1)
   * [Schritt 5: Duplikate vermeiden](#schritt-5-duplikate-vermeiden)
      + [Erklärung](#erklärung-4)
      + [Code](#code-4)
   * [Schritt 6: Das Endergebnis](#schritt-6-das-endergebnis)
      + [Erklärung](#erklärung-5)
      + [Ergebnis für unser Beispiel](#ergebnis-für-unser-beispiel)
   * [Code-Details: Ein genauerer Blick auf C\#](#code-details-ein-genauerer-blick-auf-c)
      + [Die Methode `GroupPointsByY`](#die-methode-grouppointsbyy)
         - [Was bedeuten die Befehle?](#was-bedeuten-die-befehle)
      + [Die Methode `CreateLines`](#die-methode-createlines)
         - [Was bedeuten die Befehle?](#was-bedeuten-die-befehle-1)
      + [Die Methode `FindPotentialRectangles`](#die-methode-findpotentialrectangles)
         - [Was bedeuten die Befehle?](#was-bedeuten-die-befehle-2)
         - [1\. `ConcurrentDictionary<IRectangle, bool>`**](#1-concurrentdictionaryirectangle-bool)
         - [2\. Linien neu gruppieren: `lines.GroupBy(...)`](#2-linien-neu-gruppieren-linesgroupby)
         - [3\. Die X-Werte vergleichen: Die `if`-Bedingung](#3-die-x-werte-vergleichen-die-if-bedingung)
         - [4\. Die Methode `GetMatchingXGroups`](#4-die-methode-getmatchingxgroups)
      + [Was ist das Ziel dieser Methode?](#was-ist-das-ziel-dieser-methode)
      + [Der Code](#der-code)
      + [Wie die Methode funktioniert (Schritt-für-Schritt-Beispiel)](#wie-die-methode-funktioniert-schritt-für-schritt-beispiel)
   * [Dokumentation: Die Klasse `Rectangle`](#dokumentation-die-klasse-rectangle)
      + [Die "geheimen Zutaten": `HashSeed` und `HashFactor`](#die-geheimen-zutaten-hashseed-und-hashfactor)
      + [Das "schlaue Gedächtnis": Caching](#das-schlaue-gedächtnis-caching)
      + [Die Methoden: Die Werkzeuge der Klasse](#die-methoden-die-werkzeuge-der-klasse)
         - [1\. Die Methode `GetOrderedPoints()`](#1-die-methode-getorderedpoints)
         - [2\. Die Methode `Equals()`](#2-die-methode-equals)
         - [3\. Die Methode `GetHashCode()`](#3-die-methode-gethashcode)
      + [Analyse der Schritte im Detail](#analyse-der-schritte-im-detail)
         - [Schritt 1: Prüfung des Gedächtnisses (Cache)](#schritt-1-prüfung-des-gedächtnisses-cache)
         - [Schritt 2: Der `unchecked` Block](#schritt-2-der-unchecked-block)
         - [Schritt 3: Die Start-Zutat (`HashSeed`)](#schritt-3-die-start-zutat-hashseed)
         - [Schritt 4: Das Vermischen (`foreach`-Schleife)](#schritt-4-das-vermischen-foreach-schleife)
         - [Schritt 5 & 6: Speichern und Zurückgeben](#schritt-5--6-speichern-und-zurückgeben)
   * [Dokumentation: Die Klasse `Line`](#dokumentation-die-klasse-line)
      + [Die Methode `Equals`](#die-methode-equals)
         - [1\. `public override bool Equals(object? obj)`](#1-public-override-bool-equalsobject-obj)
         - [2\. `public bool Equals(Line? other)`](#2-public-bool-equalsline-other)
      + [Die Methode `GetHashCode()`](#die-methode-gethashcode)
         - [Was ist `^` (XOR) und wie funktioniert es?](#was-ist--xor-und-wie-funktioniert-es)

<!-- TOC end -->

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

-----

## Dokumentation: Die Klasse `Rectangle`

Diese Klasse ist mehr als nur ein einfacher Behälter für zwei Linien. Sie ist eine intelligente Struktur, die sehr schnell und zuverlässig arbeiten kann. Sie weiß, wie man sich selbst mit anderen Rechtecken vergleicht und wie man sich für schnelle Suchen vorbereitet.

### Die "geheimen Zutaten": `HashSeed` und `HashFactor`

Ganz oben in der Klasse finden wir diese zwei Zeilen:

```csharp
private const int HashSeed = 19;
private const int HashFactor = 31;
```

  * **Was sind das?** Das sind zwei feste Zahlen, die wir für eine spezielle Berechnung benutzen: den `GetHashCode`. Man kann sie sich als "Start-Zutat" (`HashSeed`) und "Misch-Faktor" (`HashFactor`) vorstellen.
  * **Warum die Zahlen 19 und 31?** Das sind **Primzahlen**. Programmierer benutzen oft Primzahlen für Hash-Berechnungen. Der Grund ist mathematisch, aber die einfache Erklärung ist: Primzahlen helfen dabei, die Ergebnisse besser zu "mischen". Das reduziert die Wahrscheinlichkeit, dass zwei **verschiedene** Rechtecke zufällig denselben Hash-Code bekommen. Das macht unser `Dictionary` und `HashSet` schneller und zuverlässiger. Die genauen Zahlen sind nicht heilig, aber kleine Primzahlen sind eine gute und bewährte Wahl.

-----

### Das "schlaue Gedächtnis": Caching

Diese Klasse muss oft die gleichen, teuren Berechnungen durchführen. Um Zeit zu sparen, hat sie ein "Gedächtnis" (Cache).

```csharp
private Point[]? _cachedOrderedPoints;
private int? _cachedHashCode;
private string? _cachedString;
```

  * **Was ist das?** Das sind private Felder, die ein Ergebnis speichern, nachdem es **einmal** berechnet wurde.
  * **Wie funktioniert es?** Wenn eine Methode wie `GetHashCode()` aufgerufen wird, prüft sie zuerst: "Habe ich dieses Ergebnis schon im Gedächtnis?"
      * **Ja:** Super, ich gebe einfach den gespeicherten Wert zurück. (Sehr schnell\!)
      * **Nein:** Okay, ich berechne das Ergebnis, speichere es in meinem Gedächtnis für das nächste Mal und gebe es dann zurück. (Dauert beim ersten Mal länger.)
  * Das ist eine sehr starke Optimierungstechnik\! 🚀

-----

### Die Methoden: Die Werkzeuge der Klasse

#### 1\. Die Methode `GetOrderedPoints()`

```csharp
public IEnumerable<Point> GetOrderedPoints()
{
    if (_cachedOrderedPoints != null)
        return _cachedOrderedPoints;
    // ...
}
```

  * **Was sie tut:** Diese Methode gibt die vier Eckpunkte des Rechtecks zurück, aber immer in einer **festen, sortierten Reihenfolge**.
  * **Warum ist das wichtig?** Ein Rechteck kann auf verschiedene Weisen erstellt werden (Linie A + B oder Linie B + A). Aber am Ende ist es dasselbe Rechteck. Durch das Sortieren der Punkte (zuerst nach X, dann nach Y) stellen wir sicher, dass jedes Rechteck eine **eindeutige Identität** hat. Das ist die Grundlage für zuverlässige Vergleiche.
  * **Wie sie Caching nutzt:** Sie prüft zuerst, ob die sortierten Punkte schon im `_cachedOrderedPoints`-Gedächtnis liegen. Wenn ja, gibt sie diese sofort zurück.

#### 2\. Die Methode `Equals()`

```csharp
public bool Equals(IRectangle? other)
{
    // ...
    var thisPoints = GetOrderedPoints();
    var otherPoints = other.GetOrderedPoints();
    return thisPoints.SequenceEqual(otherPoints);
}
```

  * **Was sie tut:** Sie vergleicht, ob dieses Rechteck mit einem anderen (`other`) identisch ist.
  * **Wie sie funktioniert:** Sie ist sehr schlau. Sie sagt nicht "Sind Linie1 und Linie2 gleich?". Stattdessen fragt sie:
    1.  Gib mir die sortierten Punkte von mir selbst.
    2.  Gib mir die sortierten Punkte des anderen Rechtecks.
    3.  Sind diese beiden sortierten Listen exakt gleich? (`SequenceEqual` prüft das.)
  * Dank `GetOrderedPoints` ist dieser Vergleich **100% zuverlässig**, egal wie die Rechtecke ursprünglich erstellt wurden.

#### 3\. Die Methode `GetHashCode()`


Ein `HashCode` ist wie ein **digitaler Fingerabdruck** für ein Objekt. Jedes Objekt bekommt eine fast einzigartige Nummer. Datenstrukturen wie `HashSet` oder `Dictionary` benutzen diesen Fingerabdruck, um Objekte blitzschnell zu finden. Diese Methode stellt sicher, dass unser Fingerabdruck für das `Rectangle`-Objekt korrekt, konsistent und schnell erstellt wird.

**Der Code:**

```csharp
public override int GetHashCode()
{
    // Schritt 1: Prüfung des Gedächtnisses (Cache)
    if (_cachedHashCode.HasValue)
        return _cachedHashCode.Value;

    // Schritt 2: Der sichere Rechenbereich
    unchecked
    {
        // Schritt 3: Die Start-Zutat
        int hash = HashSeed; 

        // Schritt 4: Das Vermischen der Zutaten
        foreach (var point in GetOrderedPoints())
        {
            hash = hash * HashFactor + point.GetHashCode();
        }

        // Schritt 5: Das Ergebnis im Gedächtnis speichern
        _cachedHashCode = hash;

        // Schritt 6: Das Ergebnis zurückgeben
        return hash;
    }
}
```

-----

### Analyse der Schritte im Detail

#### Schritt 1: Prüfung des Gedächtnisses (Cache)

```csharp
if (_cachedHashCode.HasValue)
    return _cachedHashCode.Value;
```

  * **Was passiert hier?** Bevor die Methode irgendeine Arbeit macht, schaut sie in ihr "Gedächtnis" (`_cachedHashCode`). `HasValue` prüft, ob dort schon ein Wert gespeichert ist.
  * **Warum?** Die Berechnung eines Hash-Codes kann (ein bisschen) teuer sein, besonders wenn sie oft aufgerufen wird. Wenn wir den Wert schon einmal berechnet haben, gibt es keinen Grund, es nochmal zu tun.
  * **Ergebnis:** Wenn schon ein Wert da ist, wird er sofort zurückgegeben. Das ist extrem schnell. Die Methode endet hier.

-----

#### Schritt 2: Der `unchecked` Block

```csharp
unchecked
{
    // ... Berechnungen ...
}
```

  * **Was ist das?** In C\# wird standardmäßig geprüft, ob eine mathematische Operation zu einer zu großen Zahl führt (einem sogenannten "Overflow"). Wenn das passiert, gibt es einen Fehler und das Programm stürzt ab.
  * **Warum benutzen wir `unchecked`?** Bei der Berechnung von Hash-Codes **wollen** wir, dass die Zahlen überlaufen. Es ist ein Teil des "Misch"-Prozesses. Wenn die Zahl zu groß für eine `int` wird, soll sie einfach wieder am Anfang des Zahlenbereichs (im negativen Bereich) weitermachen. `unchecked` sagt dem Programm: "Schalte diese Sicherheitsprüfung für den folgenden Code-Block aus. Ich weiß, was ich tue. Kein Absturz bei einem Overflow."

-----

#### Schritt 3: Die Start-Zutat (`HashSeed`)

```csharp
int hash = HashSeed;
```

  * **Was passiert hier?** Wir deklarieren eine neue Variable namens `hash` und geben ihr einen Startwert. Dieser Startwert ist unsere Konstante `HashSeed` (die Zahl 19).
  * **Warum?** Wir brauchen einen Anfangspunkt für unsere Berechnung. Wir können nicht mit Null anfangen, weil das zu schlechteren, weniger einzigartigen Hash-Codes führen würde, besonders wenn einer der Punkte den Hash-Code Null hat. Eine Primzahl wie 19 ist ein viel besserer, "zufälligerer" Startpunkt.

-----

#### Schritt 4: Das Vermischen (`foreach`-Schleife)

```csharp
foreach (var point in GetOrderedPoints())
{
    hash = hash * HashFactor + point.GetHashCode();
}
```

  * Das ist die **Koch-Anleitung** für unseren Fingerabdruck.
  * **`foreach (var point in GetOrderedPoints())`**: Die Schleife geht durch die vier Eckpunkte des Rechtecks. Wichtig ist, dass `GetOrderedPoints()` die Punkte immer in derselben, sortierten Reihenfolge zurückgibt. Das garantiert, dass **gleiche Rechtecke immer den gleichen Hash-Code** bekommen.
  * **`hash = hash * HashFactor + point.GetHashCode();`**: Das ist die magische Formel. Bei jeder Wiederholung passiert Folgendes:
    1.  Nimm den aktuellen `hash`-Wert.
    2.  Multipliziere ihn mit unserem `HashFactor` (der Primzahl 31). Das "streckt" und verteilt die Bits der Zahl.
    3.  Hole den eigenen Fingerabdruck (`GetHashCode()`) des aktuellen `point`.
    4.  Addiere diesen Punkt-Fingerabdruck zum Ergebnis.
  * **Beispiel-Ablauf:**
      * **Start:** `hash = 19`
      * **Punkt 1:** `hash = (19 * 31) + hash_von_punkt1`
      * **Punkt 2:** `hash = (aktueller_hash * 31) + hash_von_punkt2`
      * **Punkt 3:** `hash = (aktueller_hash * 31) + hash_von_punkt3`
      * **Punkt 4:** `hash = (aktueller_hash * 31) + hash_von_punkt4`
  * Am Ende dieser Schleife haben wir eine finale Zahl, die auf einzigartige Weise von allen vier Punkten und ihrer Reihenfolge beeinflusst wurde.

-----

#### Schritt 5 & 6: Speichern und Zurückgeben

```csharp
_cachedHashCode = hash;
return hash;
```

  * **`_cachedHashCode = hash;`**: Bevor wir das Ergebnis zurückgeben, speichern wir es in unserem Gedächtnis. Beim nächsten Aufruf dieser Methode wird Schritt 1 diesen Wert finden und sofort zurückgeben.
  * **`return hash;`**: Wir geben den finalen, berechneten Fingerabdruck zurück.

-----

## Dokumentation: Die Klasse `Line`

Diese Klasse repräsentiert eine einfache Linie zwischen zwei Punkten. Aber sie hat eine sehr wichtige Eigenschaft: Die Reihenfolge der Punkte spielt keine Rolle. Eine Linie von Punkt A nach B ist dieselbe wie eine Linie von B nach A. Der Code in dieser Klasse sorgt dafür, dass das Programm das auch so versteht.

### Die Methode `Equals`

Diese Methode prüft, ob zwei Linien identisch sind. Es gibt zwei `Equals`-Methoden, schauen wir sie uns an.

#### 1\. `public override bool Equals(object? obj)`

```csharp
public override bool Equals(object? obj)
{
    return ReferenceEquals(this, obj) || (obj is Line otherLine && Equals(otherLine));
}
```

  * Diese Methode ist der allgemeine Einstiegspunkt für Vergleiche.
  * **`ReferenceEquals(this, obj)`**: Das ist der **schnellste Check**. Er fragt: "Zeigen diese beiden Variablen (`this` und `obj`) auf **exakt dasselbe Objekt** im Speicher des Computers?"
      * Wenn ja, dann sind sie zu 100% gleich. Die Methode gibt sofort `true` zurück und ist fertig. Das spart Zeit.
  * **`||` (ODER)**: Wenn `ReferenceEquals` `false` ist, geht es hier weiter.
  * **`obj is Line otherLine && Equals(otherLine)`**: Das prüft zwei Dinge:
    1.  `obj is Line otherLine`: Ist das andere Objekt (`obj`) überhaupt eine `Line`? Wenn nicht, können sie nicht gleich sein. Wenn es eine `Line` ist, wird es in der neuen Variable `otherLine` gespeichert.
    2.  `&& Equals(otherLine)`: Wenn es eine `Line` ist, wird die **andere, spezifischere `Equals`-Methode** (die wir als nächstes besprechen) aufgerufen, um den eigentlichen Wertvergleich zu machen.

#### 2\. `public bool Equals(Line? other)`

```csharp
public bool Equals(Line? other)
{
    if (other is null)
    {
        return false;
    }

    return Point1.Equals(other.Point1) && Point2.Equals(other.Point2) || 
           Point1.Equals(other.Point2) && Point2.Equals(other.Point1);
}
```

  * Das ist die **Kernlogik** des Vergleichs.
  * Die Bedingung prüft beide möglichen Anordnungen der Punkte:
      * **Teil 1:** `Point1.Equals(other.Point1) && Point2.Equals(other.Point2)`
          * Ist mein erster Punkt gleich dem ersten Punkt der anderen Linie UND mein zweiter Punkt gleich dem zweiten?
      * `||` **(ODER)**
      * **Teil 2:** `Point1.Equals(other.Point2) && Point2.Equals(other.Point1)`
          * Ist mein erster Punkt gleich dem zweiten Punkt der anderen Linie UND mein zweiter Punkt gleich dem ersten? (Der "über Kreuz"-Vergleich)
  * Wenn einer dieser beiden Teile `wahr` ist, sind die Linien gleich. Damit stellen wir sicher, dass `Line(A, B)` und `Line(B, A)` als identisch angesehen werden.

-----

### Die Methode `GetHashCode()`

```csharp
public override int GetHashCode()
{
    int hashPoint1 = Point1.GetHashCode();
    int hashPoint2 = Point2.GetHashCode();

    return hashPoint1 ^ hashPoint2;
}
```

  * Diese Methode muss eine Regel befolgen: Wenn zwei Objekte laut `Equals` gleich sind, **müssen** sie auch denselben Hash-Code haben.
  * Wie erreichen wir das, wenn die Reihenfolge der Punkte egal ist? Mit einem cleveren mathematischen Trick: dem **XOR-Operator `^`**.

#### Was ist `^` (XOR) und wie funktioniert es?

  * `^` steht für "Exklusives ODER". Es ist eine bitweise Operation. Die einfache Erklärung ist: Es ist eine Art von Addition, bei der die Reihenfolge keine Rolle spielt.
  * Es hat die Eigenschaft: `A ^ B` ist **immer** dasselbe Ergebnis wie `B ^ A`.

**Beispiel:**

  * Nehmen wir an, `Point1.GetHashCode()` gibt die Zahl **10** zurück.

  * Und `Point2.GetHashCode()` gibt die Zahl **25** zurück.

  * **Fall A:** `Line(Point1, Point2)`

      * `10 ^ 25` ergibt einen bestimmten Wert (z.B. 19).

  * **Fall B:** `Line(Point2, Point1)`

      * `25 ^ 10` ergibt **exakt denselben Wert** (19).

Dank des `^` Operators produzieren `Line(A, B)` und `Line(B, A)` denselben Hash-Code. Damit ist die Regel erfüllt und unser `HashSet` oder `Dictionary` funktioniert perfekt und zuverlässig mit unseren `Line`-Objekten. ✨
