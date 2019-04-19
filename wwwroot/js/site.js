var fetchCount = 0;
var lastTimeRangeId = "";
var lastEntryId = "";

function onJSONGet(data) {
    if (fetchCount != 0) {
        if (lastEntryId != data.entry.id) {
            location.reload()
        }
        if (lastTimeRangeId != data.range.id) {
            $("#warningArea").removeAttr("hidden");
            $("#warningArea").html("Warning: New entry detected! <br /> Time Range: " + data.range.startAt + " - " + data.range.endAt)
            setTimeout(() => { $("#warningArea").attr("hidden", "true"); }, 5000)
        }
    }

    lastEntryId = data.entry.id;
    lastTimeRangeId = data.range.id;
    fetchCount++;
}
function fetchJson() {
    $.getJSON("/Status", onJSONGet);
}
setInterval(fetchJson, 2000);
