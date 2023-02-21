function renderChart(chartType, stacked, containerId, chartDto, isAggregated, onClickCallback, options) {
    // delete all child elements and create new canvas
    // so that hover events don't mess up the view
    var container = document.getElementById(containerId);
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }
    var canvas = document.createElement("canvas");
    canvas.setAttribute('name', chartDto.label);
    container.appendChild(canvas);

    var config = {
        type: chartType,
        data: {
            labels: [],
            datasets: []
        }
    }

    data = []
    labels = []
    for (var index in chartDto.datasets) {
        var dataset = chartDto.datasets[index];

        if (isAggregated) {
            config.data.labels = Object.keys(dataset.data);
            config.data.datasets.push(
                {
                    label: dataset.label,
                    data: Object.values(dataset.data),
                    fill: true,
                    backgroundColor: [colors(0.3)[index]],
                    borderColor: [colors(0.4)[index]],
                    pointBorderColor: new Array(Object.values(dataset.data).length).fill(colors(0.7)[index], 0, Object.values(dataset.data).length),
                    pointBackgroundColor: new Array(Object.values(dataset.data).length).fill(colors(0.5)[index], 0, Object.values(dataset.data).length),
                    borderWidth: 1
                }
            );
        } else {
            data.push(dataset.data);
            labels.push(dataset.label);
        }
    }

    if (!isAggregated) {
        config.data.labels = labels;
        config.data.datasets.push(
            {
                data: data,
                backgroundColor: colors(0.3).slice(0, chartDto.datasets.length),
                borderColor: colors(0.4).slice(0, chartDto.datasets.length),
                pointBorderColor: colors(0.7).slice(0, chartDto.datasets.length),
                pointBackgroundColor: colors(0.5).slice(0, chartDto.datasets.length),
                borderWidth: 1
            }
        );
    }

    yAxes = [{
        ticks: {
            display: false
        },
        gridLines: {
            display: false,
            drawBorder: false,
        }
    }]


    if (chartType !== 'pie') {
        yAxes = [{
            ticks: {
                beginAtZero: true,
                callback: function (value) { if (value % 1 === 0) { return value; } }
            },
            gridLines: {
                display: false
            },
            stacked: stacked
        }]
    }

    if (options === undefined) {
        options = {};
    }

    config.options = {
        responsive: true,
        onHover: options.onHover !== undefined ? undefined : function (event) {
            event.target.style.cursor = 'default';
        },
        title: {
            display: false,
            text: chartDto.label
        },
        legend: {
            display: options.legend !== undefined && options.legend.display !== undefined ? options.legend.display : true,
            position: 'bottom',
            onHover: function (event, legendItem) {
                if (legendItem) {
                    event.srcElement.style.cursor = 'pointer';
                }
            }
        },
        tooltips: {
            mode: 'label',
        },
        hover: {
            mode: 'label'
        },
        scales: {
            yAxes: yAxes,
            xAxes: options.scales !== undefined && options.scales.xAxes !== undefined ? [options.scales.xAxes] : []
        },
        animation: {
            onComplete: function (animation) {
                if (animation.chart.config.data.datasets.length === 0) {
                    ctx = canvas.getContext('2d');
                    ctx.font = "24px Tahoma";
                    ctx.fillStyle = "red";
                    ctx.textAlign = "center";
                    ctx.fillText("No Data to Display", canvas.width / 2, canvas.height / 2);
                } else if (animation.chart.config.data.datasets[0].data.length === 0) {
                    ctx = canvas.getContext('2d');
                    tx.font = "24px Tahoma";
                    ctx.fillStyle = "red";
                    ctx.textAlign = "center";
                    ctx.fillText("No Data to Display", canvas.width / 2, canvas.height / 2);
                }
            }
        },
        elements: {
            point: {
                radius: 1
            }
        }
    };

    var myChart = new Chart(canvas.getContext('2d'), config);

    if (onClickCallback !== undefined) {
        canvas.addEventListener('click', (function (myChart) {
            return function (e) { onClickCallback(e, myChart); };
        })(myChart), false);
    }
}

function colors(a) {
    return [
        'rgba(75, 192, 192, ' + a + ')',
        'rgba(54, 162, 235, ' + a + ')',
        'rgba(255, 99, 132, ' + a + ')',
        'rgba(255, 206, 86, ' + a + ')',
        'rgba(153, 102, 255, ' + a + ')',
        'rgba(255, 159, 64, ' + a + ')',
        'rgba(255, 0, 0, ' + a + ')',
        'rgba(0, 255, 0, ' + a + ')',
        'rgba(0, 0, 255, ' + a + ')',
        'rgba(192, 192, 192, ' + a + ')',
        'rgba(48, 192, 192, ' + a + ')',
        'rgba(192, 48, 192, ' + a + ')',
        'rgba(192, 192, 48, ' + a + ')'
    ]
}
