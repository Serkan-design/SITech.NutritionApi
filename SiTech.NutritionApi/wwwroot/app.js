let lastText = "";

const camera = document.getElementById("camera");
const loader = document.getElementById("loader");
const resultBox = document.getElementById("resultBox");
const statusText = document.getElementById("statusText");

function openCamera() {
    camera.click();
}

// 🔥 BURASI KRİTİK (DOMContentLoaded fix)
camera.addEventListener("change", async function (e) {

    const file = e.target.files[0];
    if (!file) return;

    let formData = new FormData();
    formData.append("file", file);

    loader.classList.remove("hidden");
    statusText.innerText = "Hesaplanıyor...";
    resultBox.classList.add("hidden");

    try {
        const res = await fetch("/api/nutrition/analiz", {
            method: "POST",
            body: formData
        });

        const data = await res.json();
        console.log("DATA:", data);

        document.getElementById("cal").innerText = data.kalori;
        document.getElementById("protein").innerText = data.protein;
        document.getElementById("carbs").innerText = data.karbonhidrat;
        document.getElementById("fat").innerText = data.yag;

        lastText = `${data.yemek_adi || "Yemek"} yaklaşık ${data.kalori || 0} kalori içeriyor.`;
        document.getElementById("desc").innerText = lastText;

        renderChart(data);

        statusText.innerText = "Tamamlandı ✅";
        resultBox.classList.remove("hidden");

    } catch (err) {
        console.error(err);
        statusText.innerText = "API hatası ❌";
    }

    loader.classList.add("hidden");
});

// 🔥 CHART
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
                ]
            }]
        },
        options: {
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

// 🔊 ses
function speak() {
    const utter = new SpeechSynthesisUtterance(lastText);
    utter.lang = "tr-TR";
    speechSynthesis.speak(utter);
}