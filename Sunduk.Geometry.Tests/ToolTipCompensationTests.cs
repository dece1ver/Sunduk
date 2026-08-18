using Sunduk.Geometry;
using Sunduk.Geometry.ContourElements;
using Sunduk.Geometry.ContourElements.Base;

namespace Sunduk.Geometry.Tests;

/// <summary>
/// Числовые сверки <see cref="ToolTipCompensation"/>. Часть ожидаемых значений выведена из
/// доверенных эталонов (Arc2Component — касательный вход дуги с торца, радиус выпуклой дуги) и
/// независимым ручным расчётом (45° фаска, выпуклость по направлению). Случаи, у которых пока нет
/// внешнего эталона (острый цилиндр-дуга, дуга-дуга, внутреннее точение), помечены комментарием
/// «CAD» — их ожидаемые значения нужно подтвердить построением в CAD и подставить сюда.
/// </summary>
public class ToolTipCompensationTests
{
    private const double R = 0.8;

    // ------------------------------------------------------------------
    // Касательный вход дуги с торца (Arc2Component: StartDiamWithShift = D_start - 2*R,
    // FullRadius = R_arc + R). Это эталон, перенесённый из рабочего калькулятора «Дуга №2».
    // ------------------------------------------------------------------

    [Fact]
    public void TangentialFaceEntry_ShiftsStartDiameterAndGrowsRadius()
    {
        var contour = new List<Element>
        {
            new Point(20, 0),
            new Arc(30, -5, 5, Direction.CCW, centerX: 20, centerZ: -5),
        };

        var result = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.TurningExternal);

        // точка на торце: X 20 -> 18.4 (20 - 2*0.8), Z 0 неизменен
        Assert.Equal(18.4, result[0].X!.Value, 4);
        Assert.Equal(0, result[0].Z!.Value, 4);
        // радиус выпуклой (CCW) дуги: 5 -> 5.8
        var arc = Assert.IsType<Arc>(result[1]);
        Assert.Equal(5.8, arc.Radius, 4);
    }

    // ------------------------------------------------------------------
    // Выпуклость по направлению: внешнее точение — CCW выпуклая (радиус += R), CW вогнутая
    // (радиус -= R). Внутреннее — наоборот.
    // ------------------------------------------------------------------

    [Fact]
    public void ConcaveExternalArc_ShrinksRadius()
    {
        var contour = new List<Element>
        {
            new Point(20, 0),
            new Arc(30, -5, 5, Direction.CW, centerX: 20, centerZ: -5),
        };

        var result = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.TurningExternal);

        var arc = Assert.IsType<Arc>(result[1]);
        Assert.Equal(4.2, arc.Radius, 4);
    }

    [Fact]
    public void InternalArc_ConvexityIsInverted()
    {
        // Внутреннее точение: CCW становится вогнутой (радиус -= R)
        var contourCcw = new List<Element>
        {
            new Point(20, 0),
            new Arc(30, -5, 5, Direction.CCW, centerX: 20, centerZ: -5),
        };
        var ccw = ToolTipCompensation.Compensate(contourCcw, R, ToolNoseVector.BoringInternal);
        Assert.Equal(4.2, Assert.IsType<Arc>(ccw[1]).Radius, 4);

        // Внутреннее точение: CW становится выпуклой (радиус += R)
        var contourCw = new List<Element>
        {
            new Point(20, 0),
            new Arc(30, -5, 5, Direction.CW, centerX: 20, centerZ: -5),
        };
        var cw = ToolTipCompensation.Compensate(contourCw, R, ToolNoseVector.BoringInternal);
        Assert.Equal(5.8, Assert.IsType<Arc>(cw[1]).Radius, 4);
    }

    // ------------------------------------------------------------------
    // Прямой выровненный угол (торец -> цилиндр) — поправка не нужна, узел не двигается.
    // ------------------------------------------------------------------

    [Fact]
    public void RightAngleCorner_Unchanged()
    {
        var contour = new List<Element>
        {
            new Point(40, 0),
            new Point(30, 0),
            new Point(30, -20),
        };

        var result = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.TurningExternal);

        Assert.Equal(30, result[1].X!.Value, 4);
        Assert.Equal(0, result[1].Z!.Value, 4);
    }

    // ------------------------------------------------------------------
    // 45° фаска (торец -> цилиндр через наклонную кромку). Вместо хрупкой ручной константы
    // проверяем геометрический инвариант: скомпенсированная вершина должна лежать ровно на
    // расстоянии R от обеих исходных кромок (нос касается обеих одновременно).
    // ------------------------------------------------------------------

    [Fact]
    public void Chamfer45Corner_VertexIsTangentToBothEdges()
    {
        var contour = new List<Element>
        {
            new Point(40, 0),
            new Point(20, -10),
            new Point(20, -20),
        };

        var result = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.TurningExternal);

        // X результата — диаметр, переводим в радиус
        double vx = result[1].X!.Value / 2;
        double vz = result[1].Z!.Value;

        // Вершина = центр носика + (-R,-R); значит центр носика = вершина + (R,R) и он должен
        // лежать на расстоянии R от обеих кромок (носик касается обеих одновременно).
        double nx = vx + R;
        double nz = vz + R;

        // входящая кромка: (20,0)->(10,-10) в радиусе (45°)
        var distIn = DistanceToLine(nx, nz, 20, 0, 10, -10);
        // исходящая кромка: вертикаль X=10
        var distOut = Math.Abs(nx - 10);

        Assert.Equal(R, distIn, 6);
        Assert.Equal(R, distOut, 6);
    }

    private static double DistanceToLine(double px, double pz, double ax, double az, double bx, double bz)
    {
        double dx = bx - ax;
        double dz = bz - az;
        double cross = Math.Abs(dx * (pz - az) - dz * (px - ax));
        return cross / Math.Sqrt(dx * dx + dz * dz);
    }

    // ------------------------------------------------------------------
    // Внутреннее vs наружное на одном контуре: «центр» один и тот же, отличается только знак
    // X-компоненты офсета вершины -> X(внутр) = X(нар) + 2R (в радиусе), Z совпадает.
    // ------------------------------------------------------------------

    [Fact]
    public void InternalMirrorsExternal_SameCenterOppositeXOffset()
    {
        var contour = new List<Element>
        {
            new Point(40, 0),
            new Point(30, -10),
            new Point(20, -10),
        };

        var ext = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.TurningExternal);
        var intr = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.BoringInternal);

        // X в радиусе: разница 2R = 1.6 -> в диаметре 3.2
        Assert.Equal(intr[1].X!.Value, ext[1].X!.Value + 3.2, 4);
        Assert.Equal(intr[1].Z!.Value, ext[1].Z!.Value, 4);
    }

    // ------------------------------------------------------------------
    // Внутренний касательный вход с торца: сдвиг X в другую сторону (X += R в радиусе).
    // ------------------------------------------------------------------

    [Fact]
    public void InternalTangentialFaceEntry_ShiftsXOutward()
    {
        var contour = new List<Element>
        {
            new Point(20, 0),
            new Arc(30, -5, 5, Direction.CCW, centerX: 20, centerZ: -5),
        };

        var result = ToolTipCompensation.Compensate(contour, R, ToolNoseVector.BoringInternal);

        // точка на торце: X 20 -> 21.6 (20 + 2*0.8)
        Assert.Equal(21.6, result[0].X!.Value, 4);
    }

    // ------------------------------------------------------------------
    // ArcAnchor.Sharp: общая формула острого стыка прямая-дуга (используется и CAM-компенсацией,
    // и калькулятором «Прочее»). Проверяем на некасательном стыке торца.
    // ------------------------------------------------------------------

    [Fact]
    public void ArcAnchorSharp_NonTangentialFace()
    {
        // Торец Z=0, дуга центр (10, -3) радиус 5 (катет 3, касательная под 36.87° от Z).
        var sharp = ArcAnchor.Sharp(ArcAnchorKind.Face, 0, 10, -3, 5, R, external: true);

        Assert.NotNull(sharp);
        // px = 10 + sqrt(25-9) = 14; K(36.87°) = 0.8*(1 - 1/tan(71.565°)) = 0.8*(1 - 1/3) = 0.5333
        Assert.Equal(14 - 0.5333, sharp!.Value.X, 3);
        Assert.Equal(0, sharp.Value.Z, 6);
    }

    [Fact]
    public void ArcAnchorSharp_TangentialReturnsNull()
    {
        // Торец Z=0, дуга центр (10, -5) радиус 5 — касательный вход (катет = радиус, угол 90°).
        var sharp = ArcAnchor.Sharp(ArcAnchorKind.Face, 0, 10, -5, 5, R, external: true);
        Assert.Null(sharp);
    }
}
