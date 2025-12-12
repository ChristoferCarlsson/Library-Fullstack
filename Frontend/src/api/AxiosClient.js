import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7068/api",
});

export default api;
