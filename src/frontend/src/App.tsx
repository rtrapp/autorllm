import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import ProjectsList from "./pages/ProjectsList";
import ProjectWorkspace from "./pages/ProjectWorkspace";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/projects" element={<ProjectsList />} />
        <Route path="/projects/:id" element={<ProjectWorkspace />} />
      </Routes>
    </BrowserRouter>
  );
}
