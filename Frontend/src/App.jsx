import { useState } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import Home from "./pages/Home";
import Logo from "./components/Logo";
import DashboardSummary from "./components/DashboardSummary";

import AdminHome from "./pages/Admin/AdminHome";
import AdminBooks from "./pages/Admin/AdminBooks";
import AdminAuthors from "./pages/Admin/AdminAuthors";
import AdminMembers from "./pages/Admin/AdminMembers";

import BookDetails from "./components/BookDetails";

export default function App() {
  const [dashboardVersion, setDashboardVersion] = useState(0);
  const handleDataChanged = () => setDashboardVersion((v) => v + 1);

  return (
    <BrowserRouter>
      <Logo />
      <DashboardSummary version={dashboardVersion} />

      <Routes>
        <Route path="/" element={<Home />} />

        {/* Admin */}
        <Route path="/admin" element={<AdminHome />} />
        <Route
          path="/admin/books"
          element={<AdminBooks onDataChanged={handleDataChanged} />}
        />
        <Route
          path="/admin/authors"
          element={<AdminAuthors onDataChanged={handleDataChanged} />}
        />
        <Route
          path="/admin/members"
          element={<AdminMembers onDataChanged={handleDataChanged} />}
        />

        {/* Book details */}
        <Route
          path="/books/:id"
          element={<BookDetails onDataChanged={handleDataChanged} />}
        />
      </Routes>
    </BrowserRouter>
  );
}
