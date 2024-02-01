# RectanglesCalculator

## Description
The RectanglesCalculator is an advanced C# application designed for efficiently identifying and enumerating rectangles formed from a given set of points. It excels in spatial analysis and graphical calculations through optimized algorithms and parallel processing.

## Key Features
- **Data Optimization**: Cleanses input data by removing duplicate points to ensure accuracy and improve efficiency.
- **Algorithmic Precision**: Employs a specialized algorithm for sorting and grouping points, creating lines, and identifying potential rectangles.
- **Parallel Processing**: Utilizes multi-threading to enhance performance in line creation and rectangle detection.
- **Robust Performance**: Capable of efficiently handling large datasets while maintaining high accuracy and performance.
- **User-Friendly Interface**: Features a clear and interactive console interface for easy data loading, processing, and result viewing.
- **Versatility**: Ideal for various applications in geometric calculations and spatial data analysis.

## Technologies Used
- C#
- .NET Core
- Concurrent Collections
- LINQ

## Project Outcome
RectanglesCalculator represents a successful development of a versatile tool for analyzing complex spatial data. It demonstrates significant reductions in processing time and improvements in accuracy for rectangle detection. This project highlights my skills in handling complex algorithms, parallel processing, and user interface design in software development.


![rectangles](https://github.com/jahanalem/RectanglesCalculator/assets/3236721/c27c5a69-cdf2-4d54-9469-65fc136e85ee)

# Rectangle Detection Algorithm:

## 1. Data Cleaning:
   - Duplicate points are removed from the list of all points to improve process efficiency and prevent inaccurate results due to matching points.

## 2. Sorting and Grouping Points:
   - Points are sorted by their X value to optimize the search for potential lines.
   - Points are grouped by their Y value, which forms the basis for creating lines on the same Y level.

## 3. Line Creation:
   - For each group of points with the same Y value, lines are created between all points within this group. This is done in parallel to enhance performance.

## 4. Potential Rectangle Detection:
   - Lines are grouped by the Y value of their starting points.
   - For each baseline, it is checked whether there are matching lines on a different Y level that have the same X values (forming the opposite sides of a potential rectangle).
   - If matching lines are found, it is verified whether the X values of the endpoints of the baseline and the comparison line match. If so, a rectangle is formed.
   - This verification is conducted in parallel to increase overall efficiency.

## 5. Removing Duplicate Rectangles:
   - Since a rectangle might be recognized more than once (e.g., through different combinations of base and comparison lines), the list of potential rectangles is cleaned to remove duplicates.

## 6. Returning Results:
   - The final list of unique rectangles is returned.

## Additional Information:
   - The number of comparison operations performed is recorded to monitor the algorithm's performance.
   - The `GetMatchingXGroups` method optimizes the search for matching lines by only returning groups of lines that can potentially form a rectangle based on the X values of the base line.

