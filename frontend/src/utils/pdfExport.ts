import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import type { Report } from '../types/api';

export function exportReportPdf(report: Report, currencyCode = 'USD') {
  const doc = new jsPDF();

  doc.setFontSize(18);
  doc.text('AI Budget Spending Analyzer Report', 14, 18);
  doc.setFontSize(11);
  doc.text(`Period: ${report.period.toUpperCase()}`, 14, 28);
  doc.text(`From: ${new Date(report.fromUtc).toLocaleDateString('en-US')}`, 14, 35);
  doc.text(`To: ${new Date(report.toUtc).toLocaleDateString('en-US')}`, 14, 42);

  autoTable(doc, {
    startY: 50,
    head: [['Metric', 'Value', 'Description']],
    body: report.metrics.map((metric) => [
      metric.label,
      new Intl.NumberFormat('en-US', { style: 'currency', currency: currencyCode }).format(metric.value),
      metric.description,
    ]),
  });

  autoTable(doc, {
    startY: (doc as jsPDF & { lastAutoTable?: { finalY?: number } }).lastAutoTable?.finalY
      ? ((doc as jsPDF & { lastAutoTable: { finalY: number } }).lastAutoTable.finalY + 10)
      : 90,
    head: [['Recommendation']],
    body: report.recommendations.map((recommendation) => [recommendation]),
  });

  doc.save(`budget-report-${report.period}.pdf`);
}
