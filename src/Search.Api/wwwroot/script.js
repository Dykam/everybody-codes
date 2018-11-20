var startup = async () => {
    let tables = {
        "column3": (cam) => cam.number % 3 == 0,
        "column5": (cam) => cam.number % 5 == 0,
        "column15": (cam) => cam.number % 3 == 0 && cam.number % 5 == 0,
        "columnOther": (cam) => cam.number % 3 != 0 && cam.number % 5 != 0,
    };

    var data = await $.getJSON("/api/cameras");

    for(var tableName in tables) {
        var tableBody = $(`#${tableName} tbody`);
        var predicate = tables[tableName];
        for(var record of data.filter(r => predicate(r))) {
            tableBody.append(`<tr>
                <td>${record.number}</td>
                <td>${record.name}</td>
                <td>${record.location.latitude}</td>
                <td>${record.location.longitude}</td>
            </tr>`);
        }
    }

    var bounds = new google.maps.LatLngBounds();
    for(var record of data) {
        bounds.extend({
            lat: record.location.latitude,
            lng: record.location.longitude
        })
    }

    var map = new google.maps.Map($("#map")[0], {
        zoom: 4,
        center: bounds.getCenter()
    });
    map.fitBounds(bounds);

    for(var record of data) {
        new google.maps.Marker({
            position: {
                lat: record.location.latitude,
                lng: record.location.longitude
            },
            map: map,
            icon: {
                url: "video-camera-icon.png",
                scaledSize: new google.maps.Size(32, 32)
            }
        })
    }
};
$(() => startup())