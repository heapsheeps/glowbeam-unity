window.addEventListener("DOMContentLoaded", () => {
    const showBtn = document.getElementById("btnShowTestcard");
    const hideBtn = document.getElementById("btnHideTestcard");
    const downloadBtn = document.getElementById("btnDownloadScan");
    const scanImage = document.getElementById("scanImage");
  
    // We'll assume our server is on the same origin/port if we loaded from it:
    const BASE_URL = window.location.origin + "/api/v1";
  
    // 1) Show testcard
    showBtn.addEventListener("click", () => {
      fetch(`${BASE_URL}/display/show_testcard`, { method: "POST"})
        .then(r => {
          if (!r.ok) throw new Error("Failed to show testcard");
        })
        .catch(err => console.error(err));
    });
  
    // 2) Hide testcard
    hideBtn.addEventListener("click", () => {
      fetch(`${BASE_URL}/display/clear`, { method: "POST"})
        .then(r => {
          if (!r.ok) throw new Error("Failed to hide testcard");
        })
        .catch(err => console.error(err));
    });
  
    // 3) Load last image on page load
    checkLastImage();
  
    // 4) Download the scan data
    downloadBtn.addEventListener("click", () => {
      // Just navigate to /api/v1/scan/download_scan as a file download
      window.location.href = `${BASE_URL}/scan/download_scan`;
    });
  
    function checkLastImage() {
      // We'll do a fetch to /api/v1/scan/last_image
      // If 200, set that as the src in <img>.
      // If 404 or error, disable the "download" button and keep no image.
      fetch(`${BASE_URL}/scan/last_image`, { method: "GET" })
        .then(response => {
          if (response.ok) {
            // We have an image
            // Convert the response to a blob so we can assign it to an <img> src
            return response.blob();
          } else {
            // 404 or some error => no image
            throw new Error("No image found");
          }
        })
        .then(blob => {
          // We got an image blob
          const imageUrl = URL.createObjectURL(blob);
          scanImage.src = imageUrl;
          // Enable download button
          downloadBtn.disabled = false;
        })
        .catch(err => {
          console.warn("No last image or error retrieving it: ", err);
          // No image => disable the download button
          downloadBtn.disabled = true;
          scanImage.removeAttribute("src");
          scanImage.alt = "No scan image yet";
        });
    }
  });
  