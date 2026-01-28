import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import ProjectsList from "./pages/ProjectsList";
import ProjectWorkspace from "./pages/ProjectWorkspace";
import { PublicLayout } from "./components/layout/PublicLayout";
import { WorkspaceLayout } from "./components/layout/WorkspaceLayout";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<PublicLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/projects" element={<ProjectsList />} />
        </Route>

        <Route path="/projects/:id" element={<WorkspaceLayout />}>
          <Route index element={<ProjectWorkspace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
