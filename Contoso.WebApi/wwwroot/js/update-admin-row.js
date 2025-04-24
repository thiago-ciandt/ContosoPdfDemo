function updateAdminRow(job) {
    const liveBadge = `<span id="live-${job.jobId}" class="badge bg-danger ms-2 animate-blink">LIVE</span>`;

    const tableBody = document.querySelector("table tbody");
    let row = document.getElementById("row-" + job.jobId);
    const isNew = !row;

    const duration = job.completedAt
        ? ((new Date(job.completedAt) - new Date(job.requestedAt)) / 1000).toFixed(2) + "s"
        : "-";

    const finalTime = job.startedFinalizationAt && job.completedAt
        ? ((new Date(job.completedAt) - new Date(job.startedFinalizationAt)) / 1000).toFixed(2) + "s"
        : "-";

    const size = job.fileSizeBytes
        ? (job.fileSizeBytes / 1024 / 1024).toFixed(2) + " MB"
        : "-";

    const badgeClass = getBadgeClass(job.status);

    const downloadButton = job.downloadUrl
        ? `<a href="${job.downloadUrl}" class="btn btn-sm btn-success" target="_blank">Download</a>`
        : "";

    const newRow = document.createElement("tr");
    newRow.id = "row-" + job.jobId;
    newRow.innerHTML = `
        <td><code>${job.jobId}</code></td>
        <td class="text-center">${job.incidentIds.length}</td>
        <td class="text-center">
            <span class="badge bg-${badgeClass}">${getStatusLabel(job.status)}</span> ${liveBadge}
        </td>
        <td class="text-center">
            <div class="progress" style="height: 20px;">
                <div class="progress-bar bg-${badgeClass}" role="progressbar" style="width: ${job.progress}%" aria-valuenow="${job.progress}" aria-valuemin="0" aria-valuemax="100">
                    ${job.progress}%
                </div>
            </div>
        </td>
        <td class="text-center">${formatTime(job.requestedAt)}</td>
        <td class="text-center">${job.completedAt ? formatTime(job.completedAt) : "-"}</td>
        <td class="text-center">${duration}</td>
        <td class="text-center">${finalTime}</td>
        <td class="text-center">${size}</td>
        <td class="text-center">${downloadButton}</td>
        <td class="text-danger">${job.errorMessage ?? ""}</td>
    `;

    if (isNew) {
        tableBody.prepend(newRow);
    } else {
        row.replaceWith(newRow);
    }

    setTimeout(() => {
        const liveElement = document.getElementById(`live-${job.jobId}`);
        if (liveElement) {
            liveElement.classList.add("fade-out");
            setTimeout(() => liveElement.remove(), 500);
        }
    }, 5000);
}
