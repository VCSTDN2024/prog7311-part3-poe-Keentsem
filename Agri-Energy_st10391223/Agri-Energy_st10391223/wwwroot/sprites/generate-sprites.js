// Simple Node.js script to generate pixel art sprites
const fs = require('fs');
const { createCanvas } = require('canvas');

// Colors based on the style guide
const colors = {
    leafGreen: '#1E4D24',
    energyYellow: '#F4D03F',
    soilBrown: '#8B5E3C',
    skyBlue: '#68A4D4',
    teal: '#2e8687',
    gray: '#444444',
    purple: '#9b59b6',
    red: '#e74c3c',
    white: '#FFFFFF'
};

// Function to create a sprite
function createSprite(filename, drawFunction) {
    const canvas = createCanvas(32, 32);
    const ctx = canvas.getContext('2d');
    
    // Clear canvas with white background
    ctx.fillStyle = '#FFFFFF';
    ctx.fillRect(0, 0, 32, 32);
    
    // Draw the sprite
    drawFunction(ctx);
    
    // Save to file
    const buffer = canvas.toBuffer('image/png');
    fs.writeFileSync(filename, buffer);
    console.log(`Created ${filename}`);
}

// Create each sprite
createSprite('sprite_farmer.png', (ctx) => {
    // Farmer with hat
    ctx.fillStyle = colors.soilBrown;
    // Hat
    ctx.fillRect(8, 4, 16, 4);
    ctx.fillRect(10, 8, 12, 2);
    // Head
    ctx.fillStyle = '#FFDBAC';
    ctx.fillRect(12, 10, 8, 6);
    // Body
    ctx.fillStyle = colors.leafGreen;
    ctx.fillRect(10, 16, 12, 12);
    // Arms
    ctx.fillRect(6, 18, 4, 6);
    ctx.fillRect(22, 18, 4, 6);
});

createSprite('sprite_crop_field.png', (ctx) => {
    // Ground
    ctx.fillStyle = colors.soilBrown;
    ctx.fillRect(0, 26, 32, 6);
    
    // Crops - rows of plants
    for (let x = 4; x < 32; x += 8) {
        // Plant stalk
        ctx.fillStyle = colors.leafGreen;
        ctx.fillRect(x, 14, 2, 12);
        
        // Leaves
        ctx.fillRect(x-2, 18, 6, 2);
        ctx.fillRect(x-1, 14, 4, 2);
        
        // Grain/fruit
        ctx.fillStyle = colors.energyYellow;
        ctx.fillRect(x, 10, 2, 4);
    }
});

createSprite('sprite_energy_icon.png', (ctx) => {
    // Sun background
    ctx.fillStyle = colors.energyYellow;
    ctx.beginPath();
    ctx.arc(16, 16, 12, 0, 2 * Math.PI);
    ctx.fill();
    
    // Wind turbine
    ctx.fillStyle = colors.white;
    ctx.fillRect(15, 14, 2, 14); // pole
    
    // Blades
    ctx.fillStyle = colors.skyBlue;
    // Center hub
    ctx.fillRect(14, 12, 4, 4);
    // Three blades
    ctx.fillRect(10, 8, 4, 3);
    ctx.fillRect(22, 13, 3, 4);
    ctx.fillRect(12, 20, 4, 3);
});

createSprite('sprite_connected_ecosystem.png', (ctx) => {
    // Globe
    ctx.fillStyle = colors.skyBlue;
    ctx.beginPath();
    ctx.arc(16, 16, 12, 0, 2 * Math.PI);
    ctx.fill();
    
    // Continents
    ctx.fillStyle = colors.leafGreen;
    ctx.fillRect(10, 10, 6, 4); 
    ctx.fillRect(18, 14, 8, 6);
    ctx.fillRect(8, 18, 5, 5);
    
    // Network lines
    ctx.strokeStyle = colors.energyYellow;
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(10, 12);
    ctx.lineTo(22, 17);
    ctx.stroke();
    
    ctx.beginPath();
    ctx.moveTo(10, 20);
    ctx.lineTo(18, 16);
    ctx.stroke();
});

createSprite('sprite_clipboard_employee.png', (ctx) => {
    // Clipboard
    ctx.fillStyle = colors.teal;
    ctx.fillRect(8, 6, 16, 22);
    
    // Paper
    ctx.fillStyle = colors.white;
    ctx.fillRect(10, 10, 12, 16);
    
    // Clip
    ctx.fillStyle = colors.gray;
    ctx.fillRect(14, 4, 4, 4);
    ctx.fillRect(12, 6, 8, 2);
    
    // Lines on paper
    ctx.fillStyle = colors.gray;
    ctx.fillRect(12, 14, 8, 1);
    ctx.fillRect(12, 18, 8, 1);
    ctx.fillRect(12, 22, 8, 1);
});

createSprite('sprite_secure_lock.png', (ctx) => {
    // Lock body
    ctx.fillStyle = colors.gray;
    ctx.fillRect(10, 14, 12, 14);
    
    // Lock shackle
    ctx.fillRect(12, 8, 2, 6);
    ctx.fillRect(18, 8, 2, 6);
    ctx.fillRect(14, 8, 4, 2);
    
    // Keyhole
    ctx.fillStyle = colors.white;
    ctx.beginPath();
    ctx.arc(16, 21, 2, 0, 2 * Math.PI);
    ctx.fill();
    ctx.fillRect(15, 21, 2, 3);
});

createSprite('sprite_data_chart.png', (ctx) => {
    // Background
    ctx.fillStyle = colors.white;
    ctx.fillRect(4, 4, 24, 24);
    
    // Border
    ctx.strokeStyle = colors.gray;
    ctx.lineWidth = 1;
    ctx.strokeRect(4, 4, 24, 24);
    
    // Axes
    ctx.fillStyle = colors.gray;
    ctx.fillRect(4, 28, 24, 1);
    ctx.fillRect(4, 4, 1, 24);
    
    // Bars
    ctx.fillStyle = colors.purple;
    ctx.fillRect(8, 20, 4, 8);
    
    ctx.fillStyle = colors.skyBlue;
    ctx.fillRect(14, 12, 4, 16);
    
    ctx.fillStyle = colors.leafGreen;
    ctx.fillRect(20, 8, 4, 20);
});

createSprite('search_icon.png', (ctx) => {
    // Glass part
    ctx.strokeStyle = colors.gray;
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(14, 14, 8, 0, 2 * Math.PI);
    ctx.stroke();
    
    // Handle
    ctx.fillStyle = colors.red;
    ctx.fillRect(20, 20, 8, 3);
    ctx.fillRect(20, 20, 3, 8);
});
