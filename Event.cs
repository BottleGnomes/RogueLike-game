using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueLikeGame
{
    class Event
    {
        Scene scene;
        Playing playing;
        public int id;
        public string tag;
        public string command;
        TextBox textBox;
        Queue<TextLine> lines = new Queue<TextLine>();

        public Event(Scene scene, Playing playing, TextBox textBox, int id, string tag, string command) 
        {
            this.scene = scene;
            this.playing = playing;
            this.textBox = textBox;
            this.id = id;
            this.tag = tag;
            this.command = command;
        }
        public void addLine(TextLine newLine) { lines.Enqueue(newLine); }
        public void trigger()
        {
            switch (command.Split(' ')[0])
            {
                case "clear": { textBox.setLines(lines); scene.events.Remove(this); break; }
                case "speak":
                    {
                        Enemy enemy = playing.getEnemy(command.Split(' ')[1]);
                        if (!textBox.writing && enemy != null)
                        {
                            textBox.setLines(lines);
                            foreach (TextLine line in lines)
                            {
                                if (line.type == "dialog" ) { enemy.speak(line); }
                            }
                            scene.events.Remove(this);
                        }
                        break;
                    }
                case "next": 
                    { 
                        //!!! add code to move on to next region
                        //"hallway"
                        scene.events.Clear();
                        scene.processXML(command.Split(' ')[1]);
                        textBox.waitOutput.Clear();
                        textBox.addLines(lines);
                        break; 
                    }
                default: { if (!textBox.writing) { textBox.setLines(lines); scene.events.Remove(this); } break; }
            }
        }
    }
}
