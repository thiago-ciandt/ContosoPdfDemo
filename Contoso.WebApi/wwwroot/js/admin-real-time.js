const adminConnection = new signalR.HubConnectionBuilder()
    .withUrl("/adminHub")
    .build();

adminConnection.on("JobUpdated", function (job) {
    updateAdminRow(job);
});

adminConnection.start().then(() => {
    console.log("Admin hub connected");
});

function getBadgeClass(status) {
    console.log(status);
    if (status === "Completed" || status === 2) {
        fetch('/admin/stats')
            .then(res => res.json())
            .then(updateStatsBoxes)
            .catch(console.error);
    }

    switch (status) {

        case "Pending": return "secondary";
        case "InProgress": return "info";
        case "Completed": return "success";
        case "Failed": return "danger";
        default: return "success";
    }
}

function getStatusLabel(status) {
    switch (status) {
        case 0: return "Pending";
        case 1: return "In Progress";
        case 2: return "Completed";
        case 3: return "Failed";
        default: return "Unknown";
    }
}

function formatTime(dateString) {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('pt-BR', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false
    }).format(date);
}

function updateStatsBoxes(stats) {
    document.getElementById("avg-time").innerText =
        stats.averageTimeSeconds ? stats.averageTimeSeconds.toFixed(2) + "s" : "-";

    document.getElementById("avg-size").innerText =
        stats.averageSizeMb ? stats.averageSizeMb.toFixed(2) + " MB" : "-";

    document.getElementById("total-size").innerText =
        stats.totalSizeMb.toFixed(2) + " MB";
}