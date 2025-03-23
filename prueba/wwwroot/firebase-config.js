// Import the functions you need from the SDKs you need
import { initializeApp } from "/node_modules/firebase/app";
import { getAnalytics } from "/node_modules/firebase/analytics";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyDRlRpcu0rOedY_CpnWHKm4R1OfcxraJJU",
    authDomain: "test-fastreport.firebaseapp.com",
    databaseURL: "https://test-fastreport-default-rtdb.firebaseio.com",
    projectId: "test-fastreport",
    storageBucket: "test-fastreport.firebasestorage.app",
    messagingSenderId: "764788089279",
    appId: "1:764788089279:web:7a33be0b1992ac26b2810a",
    measurementId: "G-FC4ESS7H4V"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);
// Función para obtener datos de Firebase
window.getData = async function (ruta) {
    const database = getDatabase();
    const dbRef = ref(database, ruta);
    const snapshot = await get(dbRef);
    if (snapshot.exists()) {
        return snapshot.val();
    } else {
        console.error("No se encontraron datos en la ruta especificada");
        return null;
    }
};
