using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ST10391223 - PixelController
// Created: May 14, 2025
// This controller handles all routes for the pixel-themed views in the application
// Based on the MVC architecture pattern taught in lecture week 6

namespace Agri_Energy_st10391223.Controllers
{
    // This controller implements the card-based routing system between pixel-themed views
    // Each method corresponds to a card in the About page
    // I tried to keep the code clean and organized as per lecturer's feedback on my previous assignment
    public class PixelController : Controller
    {
        // Green Energy Marketplace View
        // This was tricky to implement with all the product comparisons
        // Uses the eco_baby.png sprite as requested in the project brief
        public IActionResult GreenEnergyMarketplace()
        {
            // TODO: Add real product data from database in future iteration
            // For now just returning the static view as discussed in class
            return View();
        }

        // Farmer Role View
        // Implements sustainable farming techniques reference center
        // Had to research real farming practices for this view to make it useful
        public IActionResult FarmerRole()
        {
            // Initially tried making this require login but lecturer suggested
            // keeping it public for the demo version
            return View();
        }

        // Educational & Training View
        // This corresponds to the Food Production card in About page
        // Struggled with making this intuitive - needed several UI revisions
        public IActionResult Educational()
        {
            // The demo data is hardcoded but in future versions this should
            // pull from a database of actual courses and learning content
            return View();
        }

        // Employee Portal View
        // Includes login form and interactive dashboard
        // Uses client-side validation as taught in week 9
        public IActionResult EmployeePortal()
        {
            // In the final version, this should check for authentication
            // and redirect to login if not authenticated
            // Simplified for the demo as discussed with lecturer
            return View();
        }

        // Data Insights View
        // The most complex view - uses Chart.js for data visualization
        // Includes the existing pie chart as required in the specifications
        public IActionResult DataInsights()
        {
            // In production, would inject real data service here
            // For now using sample chart data as discussed in tutorial
            return View();
        }

        // Smart Search View
        // Implements role-specific search functionality with filters
        // This was challenging to implement with the login requirement!  
        public IActionResult SmartSearch()
        {
            // Added role-based simulation for the demo
            // TODO: Connect to real search API in future version
            // Using client-side logic for demo purposes
            return View();
        }

        // Connected Ecosystem View
        // Shows collaboration opportunities and funding resources
        // Used progressive disclosure pattern from UI/UX lecture
        public IActionResult ConnectedEcosystem()
        {
            // Still working on the funding data integration
            // Would like to pull from real grant database in the future
            // For now using static content based on research
            return View();
        }
    }
}
