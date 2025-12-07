import { BrowserRouter, Routes, Route } from "react-router-dom";

import Home from "./pages/Home";
import Logo from "./components/Logo";
import DashboardSummary from "./components/DashboardSummary";

import AdminHome from "./pages/Admin/AdminHome";
import AdminBooks from "./pages/Admin/AdminBooks";
import AdminAuthors from "./pages/Admin/AdminAuthors";
import AdminMembers from "./pages/Admin/AdminMembers";

export default function App() {
  return (
    <BrowserRouter>
      <Logo />
      <DashboardSummary />

      <Routes>
        {/* User-facing */}
        <Route path="/" element={<Home />} />

        {/* Admin */}
        <Route path="/admin" element={<AdminHome />} />
        <Route path="/admin/books" element={<AdminBooks />} />
        <Route path="/admin/authors" element={<AdminAuthors />} />
        <Route path="/admin/members" element={<AdminMembers />} />
      </Routes>
    </BrowserRouter>
  );
}
