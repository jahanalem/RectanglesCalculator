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
