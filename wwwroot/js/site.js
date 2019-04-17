var fetchCount = 0;
var lastTimeRangeId = "";
var lastEntryId = "";

function onJSONGet(data) {
    if (fetchCount != 0) {
        if (lastEntryId != data.entry.id) {
            console.log(data);
        }
        if (lastTimeRangeId != data.range.id) {
            console.log(data.range.id);
        }
    }

    lastEntryId = data.entry.id;
    lastTimeRangeId = data.range.id;
    fetchCount++;
}
function fetchJson() {
    $.getJSON("/Status", onJSONGet);
}
setInterval(fetchJson, 3000);
