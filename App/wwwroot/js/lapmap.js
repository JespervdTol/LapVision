window.renderLap = (points, miniSectors) => {
    const canvas = document.getElementById('lapCanvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (!points || points.length === 0) return;

    const padding = 40;
    const width = canvas.width - 2 * padding;
    const height = canvas.height - 2 * padding;

    ctx.lineWidth = 3;
    ctx.lineJoin = 'round';

    const grouped = points.reduce((acc, point) => {
        const key = point.miniSectorNumber ?? 0;
        if (!acc[key]) acc[key] = [];
        acc[key].push(point);
        return acc;
    }, {});

    for (const key in grouped) {
        const sectorPoints = grouped[key];
        if (sectorPoints.length < 2) continue;

        const sector = miniSectors.find(s => s.sectorNumber == key);
        if (!sector) continue;

        let color = "gray";
        if (sector.isFasterThanBest === true) {
            color = "purple";
        } else if (sector.isFasterThanPrevious === true) {
            color = "limegreen";
        } else if (sector.isFasterThanPrevious === false) {
            color = "yellow";
        }

        ctx.strokeStyle = color;
        ctx.beginPath();
        for (let i = 0; i < sectorPoints.length; i++) {
            const x = padding + sectorPoints[i].longitude * width;
            const y = padding + (1 - sectorPoints[i].latitude) * height;

            if (i === 0) ctx.moveTo(x, y);
            else ctx.lineTo(x, y);
        }
        ctx.stroke();
    }
};