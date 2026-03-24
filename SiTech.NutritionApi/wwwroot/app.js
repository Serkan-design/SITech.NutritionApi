let lastText = "";

// 🔥 AKILLI API (local + server otomatik)
const API_BASE = window.location.origin;

const camera = document.getElementById("camera");
const loader = document.getElementById("loader");
const resultBox = document.getElementById("resultBox");
const statusText = document.getElementById("statusText");

function openCamera() {
    camera.click();
}

// 📸 FOTO SEÇİLDİ
camera.addEventListener("change", async function (e) {

    const file = e.target.files[0];
    if (!file) return;

    let formData = new FormData();
    formData.append("file", file);

    loader.classList.remove("hidden");
    statusText.innerText = "AI analiz ediyor... 🧠";
    resultBox.classList.add("hidden");

    try {
        const res = await fetch(`${API_BASE}/api/nutrition/analiz`, {
            method: "POST",
            body: formData
        });

        if (!res.ok) {
            throw new Error("API ERROR: " + res.status);
        }

        const data = await res.json();
        console.log("DATA:", data);

        document.getElementById("cal").innerText = data.kalori ?? 0;
        document.getElementById("protein").innerText = data.protein ?? 0;
        document.getElementById("carbs").innerText = data.karbonhidrat ?? 0;
        document.getElementById("fat").innerText = data.yag ?? 0;

        lastText = `${data.yemek_adi || "Yemek"} yaklaşık ${data.kalori || 0} kalori içeriyor.`;
        document.getElementById("desc").innerText = lastText;

        renderChart(data);

        statusText.innerText = "Tamamlandı ✅";
        resultBox.classList.remove("hidden");

    } catch (err) {
        console.error(err);
        statusText.innerText = "API hatası ❌";
    } finally {
        loader.classList.add("hidden");
    }

    camera.value = "";
});


// 📊 CHART
function renderChart(data) {
    const ctx = document.getElementById("chart");

    if (window.myChart) {
        window.myChart.destroy();
    }

    window.myChart = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ["Protein", "Karbonhidrat", "Yağ"],
            datasets: [{
                data: [
                    data.protein || 0,
                    data.karbonhidrat || 0,
                    data.yag || 0
                ],
                backgroundColor: [
                    "#4facfe",
                    "#f093fb",
                    "#f5576c"
                ],
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            cutout: "65%",
            plugins: {
                legend: {
                    labels: {
                        color: "white"
                    }
                }
            }
        }
    });
}


// 🔊 SES
function speak() {
    if (!lastText) return;

    const utter = new SpeechSynthesisUtterance(lastText);
    utter.lang = "tr-TR";

    speechSynthesis.cancel();
    speechSynthesis.speak(utter);
}