<script>
    const uploadBtn = document.getElementById("uploadBtn");
    const imageInput = document.getElementById("imageInput");
    const resultCard = document.getElementById("resultCard");
    const diagnosisResult = document.getElementById("diagnosisResult");
    const loadingSpinner = document.getElementById("loadingSpinner");
    const customAlert = document.getElementById("customAlert");
    const alertMessage = document.getElementById("alertMessage");

    let previousImageName = null;

    function showAlert(message) {
        alertMessage.textContent = message;
    customAlert.style.display = "block";
        setTimeout(() => {
        customAlert.style.display = "none";
        }, 4000);
    }

    function hideAlert() {
        customAlert.style.display = "none";
    }

    uploadBtn.addEventListener("click", async () => {
        const file = imageInput.files[0];

    if (!file) {
        showAlert("Iltimos, rasm tanlang.");
    return;
        }

    // Agar avvalgi rasm bilan hozirgi rasm nomi bir xil bo‘lsa
    if (previousImageName === file.name) {
        showAlert("Tashxis allaqachon qo‘yilgan.");
    return;
        }

    // 🔁 Rasm yuklash
    const formData = new FormData();
    formData.append("image", file);

    loadingSpinner.style.display = "block";
    resultCard.style.display = "none";

    try {
            const response = await axios.post("https://localhost:7253/api/upload/image", formData);
    const result = response.data;

    if (response.status === 200) {
        diagnosisResult.textContent = result.diagnosis || "Tashxis aniqlanmadi.";
    resultCard.style.display = "block";

    // Rasm nomini saqlab qo‘yamiz
    previousImageName = file.name;
            } else {
        diagnosisResult.textContent = "Tashxis olishda xatolik.";
    resultCard.style.display = "block";
            }
        } catch (error) {
        diagnosisResult.textContent = "Xatolik yuz berdi.";
    resultCard.style.display = "block";
        } finally {
        loadingSpinner.style.display = "none";
        }
    });
</script>
