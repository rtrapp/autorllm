import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Users, Map, BookOpen } from "lucide-react"
import { CharactersList } from "./components/CharactersList"
import { LocationsList } from "./components/LocationsList"
import { PlotsList } from "./components/PlotsList"

export default function WorldBuilding() {
  return (
    <div className="flex-1 h-full overflow-hidden p-6 bg-secondary/10">
      <div className="max-w-5xl mx-auto h-full flex flex-col">
        <h1 className="text-3xl font-bold tracking-tight mb-6">World Building</h1>
        
        <Tabs defaultValue="characters" className="flex-1 flex flex-col overflow-hidden">
          <TabsList className="w-full justify-start border-b rounded-none p-0 h-auto bg-transparent gap-6">
            <TabsTrigger 
              value="characters"
              className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent px-4 py-2"
            >
              <Users className="h-4 w-4 mr-2" />
              Personagens
            </TabsTrigger>
            <TabsTrigger 
              value="locations"
              className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent px-4 py-2"
            >
              <Map className="h-4 w-4 mr-2" />
              Locais
            </TabsTrigger>
            <TabsTrigger 
              value="plots"
              className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent px-4 py-2"
            >
              <BookOpen className="h-4 w-4 mr-2" />
              Plots
            </TabsTrigger>
          </TabsList>
          
          <div className="flex-1 overflow-y-auto mt-6">
            <TabsContent value="characters" className="m-0 h-full">
              <CharactersList />
            </TabsContent>
            <TabsContent value="locations" className="m-0 h-full">
               <LocationsList />
            </TabsContent>
            <TabsContent value="plots" className="m-0 h-full">
               <PlotsList />
            </TabsContent>
          </div>
        </Tabs>
      </div>
    </div>
  )
}
